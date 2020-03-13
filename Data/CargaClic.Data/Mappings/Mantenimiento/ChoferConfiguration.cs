using CargaClic.Domain.Mantenimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Mantenimiento
{
    public class TrabajadorConfiguration : IEntityTypeConfiguration<Trabajador>
    {
        public void Configure(EntityTypeBuilder<Trabajador> builder)
        {
            builder.ToTable("Trabajador","Mantenimiento");
            builder.HasKey(x=>x.id);
            builder.Property(x=>x.nombre_completo).HasMaxLength(50).IsRequired();
            builder.Property(x=>x.dni).HasMaxLength(11).IsRequired();
            builder.Property(x=>x.email).HasMaxLength(11).IsRequired();
            
            

        }
    }
}