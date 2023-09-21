using Discord;
using Discord.WebSocket;
using GrantBot.Data.Models;
using GrantBot.Data.Repositories;
using GrantBot.Models;
using GrantBot.Utils;
using Microsoft.Extensions.Configuration;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using Image = SixLabors.ImageSharp.Image;

namespace GrantBot.Services.Painters.Impl;

public class ProfileInfoPainter : IProfileInfoPainter
{
    #region Services

    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly HttpClient _httpClient;
    private readonly IAwardRepository _awardRepository;

    #endregion

    private readonly Image<Rgba32> _backgroundImage;
    private readonly Font _font;
    private readonly IDictionary<string, Image> _rankImages = new Dictionary<string, Image>();
    private readonly IDictionary<string, Image> _awardImages = new Dictionary<string, Image>();
    private readonly Point _avatarPoint;
    private readonly Point _userNamePoint;
    private readonly Point _rankPoint;
    private readonly Point _awardsPoint;
    private readonly int _awardsMaxX;
    
    public ProfileInfoPainter(
        IConfiguration configuration,
        DiscordSocketClient discordSocketClient,
        HttpClient httpClient,
        IList<RankConfig> registeredRanks,
        IList<AwardConfig> registeredAwards,
        IAwardRepository awardRepository)
    {
        _configuration = configuration;
        _discordSocketClient = discordSocketClient;
        _httpClient = httpClient;
        _awardRepository = awardRepository;

        var imageGenConfig = configuration.GetSection("image-gen");
        
        _backgroundImage = Image.Load<Rgba32>(imageGenConfig["profile-background"]);

        foreach (var rank in registeredRanks)
        {
            if (rank.Image is null)
                continue;
            
            _rankImages.Add(rank.Name, Image.Load(rank.Image));
        }
        
        foreach (var award in registeredAwards)
        {
            var awardImage = Image.Load<Rgba32>(award.Image);
            _awardImages.Add(award.Id, awardImage);
        }
        
        _avatarPoint = new Point(
            imageGenConfig.GetValue<int>("profile:avatar:x"),
            imageGenConfig.GetValue<int>("profile:avatar:y"));
        _userNamePoint = new Point(
            imageGenConfig.GetValue<int>("profile:username:x"),
            imageGenConfig.GetValue<int>("profile:username:y"));
        _rankPoint = new Point(
            imageGenConfig.GetValue<int>("profile:rank:x"),
            imageGenConfig.GetValue<int>("profile:rank:y"));
        _awardsPoint = new Point(
            imageGenConfig.GetValue<int>("profile:awards:x"),
            imageGenConfig.GetValue<int>("profile:awards:y"));
        _awardsMaxX = imageGenConfig.GetValue<int>("profile:awards:max-x");
        
        var fontSize = imageGenConfig.GetValue<int>("font:size");
        var fontStyle = imageGenConfig.GetValue<FontStyle>("font:style");
        _font = new FontCollection()
            .Add(imageGenConfig["font:path"])
            .CreateFont(fontSize, fontStyle);
    }

    public Image<Rgba32> Draw(User user, long seasonId)
    {
        var newImage = _backgroundImage.Clone();
        var discordUser = _discordSocketClient.GetUser(user.DiscordId);
        var secondaryColor = HexToColorConverter.Convert(_configuration["image-gen:font:color:secondary"]);
        var drawArea = new Rectangle(Point.Empty, new Size(newImage.Width, newImage.Height));
        
        // draw username
        newImage.Mutate(i =>
            i.DrawText(discordUser.Username, _font, secondaryColor, _userNamePoint));
        
        // draw rank image
        if (user.Rank is not null)
        {
            var rankImage = _rankImages[user.Rank];
        
            newImage.Mutate(i =>
                i.DrawImage(rankImage, _rankPoint, drawArea, 1f));
        }

        // draw avatar image
        var userAvatarImage = GetImageFromUrl(discordUser.GetAvatarUrl(ImageFormat.Png, 40));
        newImage.Mutate(i =>
            i.DrawImage(userAvatarImage, _avatarPoint, drawArea, 1f));

        // draw user awards
        DrawUserAwards(newImage, user, seasonId, drawArea);

        return newImage;
    }

    private Image GetImageFromUrl(string url)
    {
        var imageStream = _httpClient.GetStreamAsync(url);
        
        return Image.Load(imageStream.Result);
    }
    
    private void DrawUserAwards(Image background, User user1, long seasonId, Rectangle drawArea)
    {
        var x = _awardsPoint.X;
        var y = _awardsPoint.Y;
        
        foreach (var award in _awardRepository.GetFromUserBySeason(user1.Id, seasonId))
        {
            if (award.UniqueId is null)
                continue;

            var awardImage = _awardImages[award.UniqueId];
            var x1 = x;
            var y1 = y;
            background.Mutate(i =>
                i.DrawImage(awardImage, new Point(x1, y1), drawArea, 1f));

            x += awardImage.Width + 2;

            if (x <= _awardsMaxX) continue;
            x = _awardsPoint.X;
            y += awardImage.Height + 4;
        }
    }
}