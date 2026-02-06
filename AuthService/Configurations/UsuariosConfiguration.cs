using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AuthService.Models;

namespace AuthService.Configurations
{
    public class UsuariosConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnOrder(0).HasColumnType("int").UseIdentityColumn();
            builder.Property(x => x.Nome).HasColumnOrder(1).HasColumnType("varchar(200)").IsRequired();
            builder.Property(x => x.Email).HasColumnOrder(2).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.SenhaHash).HasColumnOrder(3).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Perfil).HasColumnOrder(4).HasColumnType("varchar(250)").IsRequired();
        }
    }
}
