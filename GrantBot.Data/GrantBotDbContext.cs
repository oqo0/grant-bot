using Microsoft.EntityFrameworkCore;

namespace GrantBot.Data;

public class GrantBotDbContext : DbContext
{
    public GrantBotDbContext(DbContextOptions options) : base(options) { }
}