using AlertaService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlertaService.Configurations
{
    public class AlertaConfiguration : IEntityTypeConfiguration<Alerta>
    {
        public void Configure(EntityTypeBuilder<Alerta> builder)
        {
            builder.ToTable("Alertas");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnOrder(0).HasColumnType("int").UseIdentityColumn();
            builder.Property(x => x.TalhaoId).HasColumnOrder(1).HasColumnType("int").IsRequired();
            builder.Property(x => x.Mensagem).HasColumnOrder(2).HasColumnType("varchar(500)").IsRequired();
            builder.Property(x => x.UmidadeSolo).HasColumnOrder(3).HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.Temperatura).HasColumnOrder(4).HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.Vento).HasColumnOrder(5).HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.Chuva).HasColumnOrder(6).HasPrecision(10, 2).IsRequired();
            builder.Property(x => x.DataAlerta).HasColumnOrder(7).HasColumnType("datetime").IsRequired();
            builder.Property(x => x.TipoAlerta).HasColumnOrder(8).HasColumnType("varchar(20)").IsRequired();
        }
    }
}
