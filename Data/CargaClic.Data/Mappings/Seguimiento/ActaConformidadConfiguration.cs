

using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Seguimiento
{
    public class ActaConformidadConfiguration: IEntityTypeConfiguration<ActaConformidad>
    {
        public void Configure(EntityTypeBuilder<ActaConformidad> builder)
        {
            builder.ToTable("ActaConformidad","Seguimiento");

        }
    }
}