using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrantBot.Modules;

public class UserModule : DiscordClientService
{
    private readonly IUserRepository _userRepository;
    private readonly IList<Rank> _registeredRanks;
    private readonly IConfiguration _configuration;

    public UserModule(
        DiscordSocketClient client,
        IUserRepository userRepository,
        ILogger<PlayingGameModule> logger,
        IConfiguration configuration
        ) : base(client, logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        
        _registeredRanks = _configuration.GetSection("ranks").Get<List<Rank>>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);

        // Since there is no UserRolesUpdated we use GuildMemberUpdated
        Client.GuildMemberUpdated += HandleGuildMemberUpdated;
    }
    
    private async Task HandleGuildMemberUpdated(Cacheable<SocketGuildUser, ulong> before, SocketGuildUser afterUser)
    {
        var user = _userRepository.GetByDiscordId(afterUser.Id);

        if (user is null)
        {
            user = new User
            {
                DiscordId = afterUser.Id,
                Rank = null
            };

            _userRepository.Create(user);
        }
        
        var userRanks = _registeredRanks
            .Where(rank => afterUser.Roles.Any(x => x.Id == rank.Id));
        var hightestRank = userRanks.MaxBy(rank => rank.Priority);

        if (user.Rank == hightestRank?.Name)
            return;
        
        user.Rank = hightestRank?.Name;

        _userRepository.Update(user);
        
        Logger.LogInformation(
            "User {UserName} ({UserId}) got a new rank {RankName}.",
            afterUser, afterUser.Id, hightestRank?.Name);
    }
}