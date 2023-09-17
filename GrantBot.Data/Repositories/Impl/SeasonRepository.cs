using GrantBot.Data.Models;

namespace GrantBot.Data.Repositories.Impl;

public class SeasonRepository : ISeasonRepository
{
    private readonly GrantBotDbContext _context;

    public SeasonRepository(GrantBotDbContext context)
    {
        _context = context;
    }
    
    public long Create(Season data)
    {
        _context.Add(data);
        _context.SaveChanges();

        return data.Id;
    }

    public Season? GetById(long id)
    {
        return _context.Seasons.FirstOrDefault(x => x.Id == id);
    }

    public IList<Season> GetAll()
    {
        return _context.Seasons.ToList();
    }

    public Season? GetCurrentSeason()
    {
        return _context.Seasons.OrderBy(season => season.StartDateTime).LastOrDefault();
    }
    
    public bool Update(Season data)
    {
        var season = GetById(data.Id);

        if (season is null)
            return false;

        season.Awards = data.Awards;
        season.StartDateTime = data.StartDateTime;
        season.Name = data.Name;

        int amountOfChanges = _context.SaveChanges();

        return amountOfChanges != 0;
    }

    public bool Delete(long id)
    {
        var season = GetById(id);

        if (season is null)
            return false;

        _context.Seasons.Remove(season);
        _context.SaveChanges();

        return true;
    }
}