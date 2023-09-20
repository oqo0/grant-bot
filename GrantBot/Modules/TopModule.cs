using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Data.Repositories;
using Microsoft.Extensions.Configuration;

namespace GrantBot.Modules;

[RequireContext(ContextType.Guild)]
[EnabledInDm(false)]
public class TopModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IUserRepository _userRepository;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IConfiguration _configuration;
    
    public TopModule(
        IUserRepository userRepository,
        DiscordSocketClient discordSocketClient,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _discordSocketClient = discordSocketClient;
        _configuration = configuration;
    }

    [SlashCommand("top", "Shows top for a current season.")]
    public async Task Top()
    {
        var topUsers = _userRepository.GetTopUsers(10);

        var embedBuilder = new EmbedBuilder()
            .WithColor(Discord.Color.Default)
            .WithTitle(_configuration["lang:award:top"]);

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