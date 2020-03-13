using CargaClic.Domain.Mantenimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Mantenimiento
{
    public class ContratistaConfiguration : IEntityTypeConfiguration<Contratista>
    {
        public void Configure(EntityTypeBuilder<Contratista> builder)
        {
            builder.ToTable("Contratista","Mantenimiento");
            builder.HasKey(x=>x.id);
            builder.Property(x=>x.razon_social).IsRequired();
        }
    }
}