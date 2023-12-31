using GrantBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GrantBot.Data.Repositories.Impl;

public class UserRepository : IUserRepository
{
    private readonly GrantBotDbContext _context;

    public UserRepository(GrantBotDbContext context)
    {
        _context = context;
    }
    
    public long Create(User data)
    {
        _context.Add(data);
        _context.SaveChanges();

        return data.Id;
    }

    public User? GetById(long id)
    {
        return _context.Users.FirstOrDefault(x => x.Id == id);
    }

    public User? GetByDiscordId(ulong userDiscordId)
    {
        return _context.Users.FirstOrDefault(x => x.DiscordId == userDiscordId);
    }

    public IList<User> GetTopUsers(int amount)
    {
        return _context.Users
            .Include(u => u.Awards)
            .OrderBy(u => u.Awards.Sum(a => a.Weight))
            .Take(amount)
            .ToList();
    }

    public IList<User> GetTopUsersBySeason(int amount, long seasonId)
    {
        return _context.Users
            .Include(u => u.Awards)
            .OrderBy(u => u.Awards
                .Where(a => a.Season.Id == seasonId)
                .Sum(a => a.Weight))
            .Take(amount)
            .ToList();
    }

    public IList<User> GetAll()
    {
        return _context.Users.ToList();
    }

    public bool Update(User data)
    {
        var user = GetById(data.Id);

        if (user is null)
            return false;

        user.Awards = data.Awards;
        user.Rank = data.Rank;

        int amountOfChanges = _context.SaveChanges();

        return amountOfChanges != 0;
    }

    public bool Delete(long id)
    {
        var user = GetById(id);

        if (user is null)
            return false;

        _context.Users.Remove(user);
        _context.SaveChanges();

        return true;
    }
}