using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Data.Repositories;
using Microsoft.Extensions.FileProviders;

namespace GrantBot.Modules;

[RequireContext(ContextType.Guild)]
[EnabledInDm(false)]
public class TopModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IUserRepository _userRepository;
    private readonly DiscordSocketClient _discordSocketClient;
    
    public TopModule(
        IUserRepository userRepository,
        DiscordSocketClient discordSocketClient)
    {
        _userRepository = userRepository;
        _discordSocketClient = discordSocketClient;
    }

    [SlashCommand("top", "Shows top for a current season.")]
    public async Task Top()
    {
        var topUsers = _userRepository.GetTopUsers(10);

        var embedBuilder = new EmbedBuilder()
            .WithColor(Discord.Color.Default)
            .WithTitle("Top 10 users by medals");

        foreach (var user in topUsers)
        {
            string userName = _discordSocketClient.GetUser(user.DiscordId).Username;
            string userRank = user.Rank ?? "None";
            embedBuilder.AddField(f =>
            {
                f.Name = userName;
                f.Value = $"{userRank}";
                f.IsInline = true;
            });
        }

        await RespondAsync(embeds: new [] { embedBuilder.Build() });
    }
}