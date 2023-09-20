using System.Reflection;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Data.Models;
using GrantBot.Models;
using GrantBot.Utils.TypeConverters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrantBot;

internal class InteractionHandler : DiscordClientService
{
    private readonly IServiceProvider _provider;
    private readonly InteractionService _interactionService;
    private readonly IConfiguration _configuration;

    public InteractionHandler(
        DiscordSocketClient client,
        ILogger<DiscordClientService> logger,
        IServiceProvider provider,
        InteractionService interactionService,
        IConfiguration configuration
        ) : base(client, logger)
    {
        _provider = provider;
        _interactionService = interactionService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Client.InteractionCreated += HandleInteraction;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        await Client.WaitForReadyAsync(stoppingToken);

        _interactionService.AddTypeConverter<AwardConfig>(new AwardConfigTypeConverter());
        _interactionService.AddTypeConverter<Season>(new SeasonTypeConverter());
        
        var serverId = _configuration.GetValue<ulong>("server-id");
        await _interactionService.RegisterCommandsToGuildAsync(serverId);
    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            await HandleInteractionProblem(interaction);
        }
        catch (Exception exception)
        {
            await HandleInteractionError(interaction, exception);
        }
    }

    private async Task HandleInteractionProblem(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(Client, interaction);
        var result = await _interactionService.ExecuteCommandAsync(context, _provider);

        if (!result.IsSuccess)
        {
            Logger.LogWarning(
                "Problem occured while {User} ({UserId}) used interaction {InteractionId}: {Problem}",
                context.User.Username, context.User.Id, interaction.Id, result.Error.ToString());
        }
    }

    private async Task HandleInteractionError(SocketInteraction interaction, Exception exception)
    {
        if (interaction.Type is InteractionType.ApplicationCommand)
        {
            await interaction.GetOriginalResponseAsync()
                .ContinueWith(async msg=> await msg.Result.DeleteAsync());

            await interaction.FollowupAsync(
                _configuration["lang:error"], ephemeral: true);
        }

        Logger.LogError(
            "Interaction {InteractionId} error occured: {Exception}", 
            interaction.Id, exception.ToString());
    }
}