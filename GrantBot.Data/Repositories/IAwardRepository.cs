using GrantBot.Data.Models;

namespace GrantBot.Data.Repositories;

public interface IAwardRepository : IRepository<long, Award>
{
    public IList<Award> GetFromUserBySeason(long userId, long seasonId);
}