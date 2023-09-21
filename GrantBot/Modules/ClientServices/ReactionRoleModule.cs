using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrantBot.Modules.ClientServices;

public class ReactionRoleModule : DiscordClientService
{
    private readonly IConfiguration _configuration;

    public ReactionRoleModule(
        DiscordSocketClient client,
        ILogger<PlayingGameModule> logger,
        IConfiguration configuration
    ) : base(client, logger)
    {
        _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        
        Client.ReactionAdded += HandleReactionAdded;
    }
    
    private async Task HandleReactionAdded(
        Cacheable<IUserMessage, ulong> message,
        Cacheable<IMessageChannel, ulong> channel,
        SocketReaction reaction)
    {
        var moduleIsEnabled = _configuration.GetValue<bool>("reaction-role:enabled");
        var messageId = _configuration.GetValue<ulong>("reaction-role:message-id");
        var emoteName = _configuration.GetValue<string>("reaction-role:emote-name");

        if (!moduleIsEnabled || message.Id != messageId || reaction.Emote.Name != emoteName)
            return;

        var roleId = _configuration.GetValue<ulong>("reaction-role:role-id");
        
        if (reaction.User.Value is SocketGuildUser socketUser)
            await socketUser.AddRoleAsync(roleId);
        
        Logger.LogInformation(
            "User {Username} ({UserId}) received role {RoleId} by using a reaction.",
            reaction.User.Value.Username, reaction.User.Value.Id, roleId);
    }
}