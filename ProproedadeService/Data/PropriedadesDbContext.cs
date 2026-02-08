using Microsoft.EntityFrameworkCore;
using PropriedadeService.Models;
using PropriedadeService.Configurations;

namespace PropriedadeService.Data
{
    public class PropriedadesDbContext : DbContext
    {
        public PropriedadesDbContext(DbContextOptions<PropriedadesDbContext> options) : base(options) { }

        public DbSet<Propriedade> Propriedades { get; set; }
        public DbSet<Talhao> Talhoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PropriedadesConfiguration());
            modelBuilder.ApplyConfiguration(new TalhaoConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
