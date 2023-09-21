using GrantBot.Data.Models;

namespace GrantBot.Services.Painters;

public interface IProfileInfoPainter
{
    public Image<Rgba32> Draw(User user, long seasonId);
}