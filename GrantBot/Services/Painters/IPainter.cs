namespace GrantBot.Services.Painters;

public interface IPainter<TId>
{
    public Image<Rgba32> Draw(TId awardId);
}