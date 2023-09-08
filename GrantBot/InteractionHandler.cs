using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace GrantBot;

public class InteractionHandler
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _handler;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InteractionHandler> _logger;

    public InteractionHandler(
        DiscordSocketClient client,
        InteractionService handler,
        IServiceProvider services,
        IConfiguration config,
        ILogger<InteractionHandler> logger)
    {
        _client = client;
        _handler = handler;
        _services = services;
        _configuration = config;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        _client.Ready += ReadyAsync;
        _handler.Log += Program.LogAsync;

        await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        _client.InteractionCreated += HandleInteraction;
    }

    private async Task ReadyAsync()
    {
        ulong serverId = _configuration.GetValue<ulong>("server-id");
        await _handler.RegisterCommandsToGuildAsync(serverId, true);
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
        var context = new SocketInteractionContext(_client, interaction);
        var result = await _handler.ExecuteCommandAsync(context, _services);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
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
                "An error occurred during the execution of the module.", ephemeral: true);
        }

        _logger.LogError(
            "Interaction {InteractionId} error occured: {Exception}", 
            interaction.Id, exception.ToString());
    }
}