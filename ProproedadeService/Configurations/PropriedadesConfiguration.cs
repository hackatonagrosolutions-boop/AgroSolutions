using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropriedadeService.Models;

namespace PropriedadeService.Configurations
{
    public class PropriedadesConfiguration : IEntityTypeConfiguration<Propriedade>
    {
        public void Configure(EntityTypeBuilder<Propriedade> builder)
        {
            builder.ToTable("Propriedades");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnOrder(0).HasColumnType("int").UseIdentityColumn();
            builder.Property(x => x.Nome).HasColumnOrder(1).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Cidade).HasColumnOrder(2).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Estado).HasColumnOrder(3).HasColumnType("char(2)").IsRequired();
            builder.Property(x => x.AreaHectares).HasColumnOrder(4).HasPrecision(18, 2).IsRequired();

            // Relacionamento 1:N (Uma Propriedade tem muitos Talhões)
            builder.HasMany(x => x.Talhoes).WithOne(t => t.Propriedade).HasForeignKey(t => t.PropriedadeId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
