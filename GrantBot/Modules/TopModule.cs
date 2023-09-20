using System.ComponentModel;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.AutocompleteHandlers;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Utils.TypeConverters;
using Microsoft.Extensions.Configuration;
using SmartFormat;

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

    [SlashCommand("top", "Shows top by season.")]
    public async Task Top([Autocomplete(typeof(SeasonAutocompleteHandler)),
                           TypeConverter(typeof(SeasonTypeConverter))] Season? season = null)
    {
        int topSize = _configuration.GetValue<int>("users-in-top");

        IList<User> topUsers;
        
        if (season is null)
            topUsers = _userRepository.GetTopUsers(topSize);
        else 
            topUsers = _userRepository.GetTopUsersBySeason(topSize, season.Id);

        var embedBuilder = PrepareEmbedBuilder(topUsers, season);
        await RespondAsync(embeds: new [] { embedBuilder.Build() });
    }

    private EmbedBuilder PrepareEmbedBuilder(IList<User> topUsers, Season? season)
    {
        var embedBuilder = new EmbedBuilder()
            .WithColor(Discord.Color.Default)
            .WithTitle(Smart.Format(
                _configuration["lang:award:top"] + (season is null ? _configuration["lang:award:in-season"] : ""),
                new 
                { 
                    TopSize = topUsers.Count, 
                    SeasonName = season?.Name 
                }));

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

        return embedBuilder;
    }
}