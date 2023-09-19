namespace GrantBot.Services.Painters;

public interface IPainter<TId>
{
    public void Generate(TId awardId);
}