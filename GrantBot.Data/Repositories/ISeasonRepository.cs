using GrantBot.Data.Models;

namespace GrantBot.Data.Repositories;

public interface ISeasonRepository : IRepository<long, Season>
{
    public Season? GetCurrentSeason();
}