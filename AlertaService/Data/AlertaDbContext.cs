using AlertaService.Models;
using Microsoft.EntityFrameworkCore;

namespace AlertaService.Data
{
    public class AlertaDbContext : DbContext
    {
        public AlertaDbContext(DbContextOptions<AlertaDbContext> options) : base(options) { }
        public DbSet<Alerta> Alertas { get; set; }
    }
}
