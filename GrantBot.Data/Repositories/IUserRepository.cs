using GrantBot.Data.Models;

namespace GrantBot.Data.Repositories;

public interface IUserRepository : IRepository<long, User>
{
    public User? GetByDiscordId(ulong userDiscordId);
    public IList<User> GetTopUsers(int amount);
    public IList<User> GetTopUsersBySeason(int amount, long seasonId);
}