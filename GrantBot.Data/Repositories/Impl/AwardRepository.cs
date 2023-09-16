using GrantBot.Data.Models;

namespace GrantBot.Data.Repositories.Impl;

public class AwardRepository : IAwardRepository
{
    private readonly GrantBotDbContext _context;

    public AwardRepository(GrantBotDbContext context)
    {
        _context = context;
    }
    
    public long Create(Award data)
    {
        _context.Add(data);
        _context.SaveChanges();

        return data.Id;
    }

    public Award? GetById(long id)
    {
        return _context.Awards.FirstOrDefault(x => x.Id == id);
    }

    public IList<Award> GetAll()
    {
        return _context.Awards.ToList();
    }

    public bool Update(Award data)
    {
        var award = GetById(data.Id);

        if (award is null)
            return false;
        
        award.Season = data.Season;
        award.User = data.User;
        award.SeasonReceivedIn = data.SeasonReceivedIn;
        award.OwnerId = data.OwnerId;
        award.Medal = data.Medal;
        award.ReceivedDateTime = data.ReceivedDateTime;

        int amountOfChanges = _context.SaveChanges();

        return amountOfChanges != 0;
    }

    public bool Delete(long id)
    {
        var award = GetById(id);

        if (award is null)
            return false;

        _context.Awards.Remove(award);
        _context.SaveChanges();

        return true;
    }
}