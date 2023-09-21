using GrantBot.Models;
using GrantBot.Utils;
using Microsoft.Extensions.Configuration;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace GrantBot.Services.Painters.Impl;

public class AwardReceivedPainter : IAwardReceivedPainter
{
    #region Services

    private readonly IConfiguration _configuration;

    #endregion
    
    private readonly Image<Rgba32> _backgroundImage;
    private readonly IDictionary<string, Image<Rgba32>> _awardsBadges;
    private readonly Font _font;
    private readonly Point _awardNamePoint;
    private readonly Point _descriptionPoint;
    private readonly Point _awardBadgePoint;
    
    public AwardReceivedPainter(IConfiguration configuration, IList<AwardConfig> registeredAwards)
    {
        _configuration = configuration;
        
        var imageGenConfig = configuration.GetSection("image-gen");
        
        _backgroundImage = Image.Load<Rgba32>(imageGenConfig["award-received-background"]);
        _awardsBadges = GetAwardBadges(registeredAwards);

        _awardNamePoint = new Point(
            imageGenConfig.GetValue<int>("new-award:name:x"),
            imageGenConfig.GetValue<int>("new-award:name:y"));
        _descriptionPoint = new Point(
            imageGenConfig.GetValue<int>("new-award:description:x"),
            imageGenConfig.GetValue<int>("new-award:description:y"));
        _awardBadgePoint = new Point(
            imageGenConfig.GetValue<int>("new-award:badge:x"),
            imageGenConfig.GetValue<int>("new-award:badge:y"));
        
        var fontSize = imageGenConfig.GetValue<int>("font:size");
        var fontStyle = imageGenConfig.GetValue<FontStyle>("font:style");
        _font = new FontCollection()
            .Add(imageGenConfig["font:path"])
            .CreateFont(fontSize, fontStyle);
    }
    
    public Image<Rgba32> Draw(AwardConfig award)
    {
        var awardImage = _awardsBadges[award.Id];
        var newImage = _backgroundImage.Clone();

        var primaryColor = HexToColorConverter.Convert(_configuration["image-gen:font:color:primary"]);
        var secondaryColor = HexToColorConverter.Convert(_configuration["image-gen:font:color:secondary"]);
        var descriptionText = _configuration["lang:award:received-image"];
        
        // draw an award name
        newImage.Mutate(i =>
            i.DrawText(award.Name, _font, primaryColor, _awardNamePoint));

        // draw the description
        newImage.Mutate(i => 
            i.DrawText(descriptionText, _font, secondaryColor, _descriptionPoint));

        // draw the award badge image
        var drawArea = new Rectangle(Point.Empty, new Size(newImage.Width, newImage.Height));
        newImage.Mutate(i =>
            i.DrawImage(awardImage, _awardBadgePoint, drawArea, 1f));

        return newImage;
    }
    
    private static IDictionary<string, Image<Rgba32>> GetAwardBadges(IList<AwardConfig> registeredAwards)
    {
        var result = new Dictionary<string, Image<Rgba32>>();
        
        foreach (var award in registeredAwards)
        {
            var awardImage = Image.Load<Rgba32>(award.Badge);
            result.Add(award.Id, awardImage);
        }

        return result;
    }
}