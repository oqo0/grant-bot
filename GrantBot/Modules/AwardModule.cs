using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Models;
using GrantBot.Services;
using GrantBot.Services.Painters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Formats.Png;
using SmartFormat;

namespace GrantBot.Modules;

[RequireContext(ContextType.Guild)]
[EnabledInDm(false)]
public class AwardModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly IUserRepository _userRepository;
    private readonly ISeasonRepository _seasonRepository;
    private readonly IAwardRepository _awardRepository;
    private readonly ILogger<SeasonModule> _logger;
    private readonly IConfiguration _configuration;
    private readonly IList<AwardConfig> _registeredAwards;
    private readonly IAwardReceivedPainter _awardReceivedPainter;

    public AwardModule(
        IUserRepository userRepository,
        ISeasonRepository seasonRepository,
        IAwardRepository awardRepository,
        ILogger<SeasonModule> logger,
        IConfiguration configuration,
        IList<AwardConfig> registeredAwards,
        IAwardReceivedPainter awardReceivedPainter)
    {
        _userRepository = userRepository;
        _seasonRepository = seasonRepository;
        _awardRepository = awardRepository;
        _logger = logger;
        _configuration = configuration;
        _registeredAwards = registeredAwards;
        _awardReceivedPainter = awardReceivedPainter;
    }

    [SlashCommand("award", "Gives an award to a specific user.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task AwardUser(
        SocketGuildUser awardReceiver,
        [Autocomplete(typeof(AwardAutocompleteHandler))] string awardId,
        bool pingUser)
    {
        var awardConfig = _registeredAwards.FirstOrDefault(a => a.Id == awardId);

        if (awardConfig is null)
        {
            await RespondAsync(_configuration["lang:award:not-found"], ephemeral: true);
            return;
        }

        var currentSeason = _seasonRepository.GetCurrentSeason();

        if (currentSeason is null)
        {
            await RespondAsync(_configuration["lang:season:not-found"], ephemeral: true);
            return;
        }
        
        var user = _userRepository.GetByDiscordId(Context.User.Id) ?? new User
        {
            Rank = null,
            DiscordId = awardReceiver.Id
        };

        var newAward = new Award
        {
            UniqueId = awardId,
            Medal = awardId,
            ReceivedDateTime = DateTime.UtcNow,
            Season = currentSeason,
            User = user
        };

        _awardRepository.Create(newAward);

        var respond = Smart.Format(
            _configuration["lang:award:gave"],
            new { AwardName = newAward.Medal, UserName = awardReceiver.Username });
        await RespondAsync(respond, ephemeral: true);
        
        _logger.LogInformation(
            "User {UserName} ({UserId}) gave an award {AwardId} to user {ReceiverName} ({ReceiverId}).",
            awardReceiver.Username, awardReceiver.Id, newAward.UniqueId, awardReceiver.Username,
            awardReceiver.Id);

        if (!pingUser)
            return;
            
        using var stream = new MemoryStream();
        var awardReceivedImage = _awardReceivedPainter.Draw(awardConfig);
        await awardReceivedImage.SaveAsync(stream, new PngEncoder());
    
        await FollowupWithFileAsync(
            new FileAttachment(stream, "newAward.png"),
            Smart.Format(_configuration["lang:award:received"],
                new { UserPing = awardReceiver.Mention, MedalName = newAward.Medal }));
    }

    [SlashCommand("awards", "Show awards of a user from a season.")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task Awards(SocketGuildUser guildUser, string season)
    {
        throw new NotImplementedException();
    }
}