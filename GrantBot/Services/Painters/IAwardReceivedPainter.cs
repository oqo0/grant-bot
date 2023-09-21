using GrantBot.Models;

namespace GrantBot.Services.Painters;

public interface IAwardReceivedPainter
{
    public Image<Rgba32> Draw(AwardConfig awardId);
}