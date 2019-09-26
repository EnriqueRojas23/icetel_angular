

using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Prerecibo
{
    public class CargaMasivaConfiguration: IEntityTypeConfiguration<CargaMasiva>
    {
        public void Configure(EntityTypeBuilder<CargaMasiva> builder)
        {
            builder.ToTable("Carga","Seguimiento");
      
        }
    }
}