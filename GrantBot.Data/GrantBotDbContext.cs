using GrantBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GrantBot.Data;

public class GrantBotDbContext : DbContext
{
    public DbSet<Award> Awards { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<User> Users { get; set; }
    
    public GrantBotDbContext(DbContextOptions<GrantBotDbContext> options) : base(options) { }
}