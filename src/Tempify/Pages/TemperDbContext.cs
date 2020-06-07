using Microsoft.EntityFrameworkCore;

namespace Codeuctivity.Pages
{
  public class TemperDbContext : DbContext
  {
    public DbSet<MesureValue> MesureValues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite("Data Source=Tempify.db");
  }
}