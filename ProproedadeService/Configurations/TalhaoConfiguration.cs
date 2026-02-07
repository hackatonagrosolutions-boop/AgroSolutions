using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropriedadeService.Models;

namespace PropriedadeService.Configurations
{
    public class TalhaoConfiguration : IEntityTypeConfiguration<Talhao>
    {
        public void Configure(EntityTypeBuilder<Talhao> builder)
        {
            builder.ToTable("Talhoes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnOrder(0).HasColumnType("int").UseIdentityColumn();
            builder.Property(x => x.Nome).HasColumnOrder(1).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Cultura).HasColumnOrder(2).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Status).HasColumnOrder(3).HasColumnType("varchar(50)").IsRequired().HasDefaultValue("Normal"); // Define o valor padrão como "Normal"
            builder.Property(x => x.AreaHectares).HasColumnOrder(4).HasPrecision(18, 2).IsRequired();

            // Relacionamento
            builder.Property<int>("PropriedadeId").HasColumnOrder(5).IsRequired();
            builder.HasOne(x => x.Propriedade).WithMany(p => p.Talhoes).HasForeignKey("PropriedadeId").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
