using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrantBot.Modules;

public class ReactionRoleModule
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReactionRoleModule> _logger;

    public ReactionRoleModule(
        DiscordSocketClient client,
        IConfiguration configuration,
        ILogger<ReactionRoleModule> logger)
    {
        _client = client;
        _configuration = configuration;
        _logger = logger;
        
        _client.ReactionAdded += HandleReactionAdded;
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
        
        _logger.LogInformation(
            "User {Username} ({UserId}) received role {RoleId} by using a reaction.",
            reaction.User.Value.Username, reaction.User.Value.Id, roleId);
    }
}