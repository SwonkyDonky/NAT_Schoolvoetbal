using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
	public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Gamble> Gambles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
        optionsBuilder.UseMySql("server=localhost;database=schoolvoetbal_nat;user=root;",
        new MySqlServerVersion(new Version(8, 0, 30)));
    }
}	