using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace GrantBot.Modules;

public class ReactionRoleModule
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public ReactionRoleModule(DiscordSocketClient client, IConfiguration configuration)
    {
        _client = client;
        _configuration = configuration;
        
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
    }
}