using Microsoft.EntityFrameworkCore;


namespace Fortune_Teller_Service.Models
{
    public class FortuneContext : DbContext
    {
        public FortuneContext(DbContextOptions<FortuneContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("fortunes", null); 
        }

        public DbSet<FortuneEntity> Fortunes { get; set; }
    }
}
