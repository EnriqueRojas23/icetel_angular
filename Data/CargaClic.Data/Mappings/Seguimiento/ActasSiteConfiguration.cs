

using CargaClic.Domain.Seguimiento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargaClic.Data.Mappings.Prerecibo
{
    public class ActasSiteConfiguration: IEntityTypeConfiguration<ActasSite>
    {
        public void Configure(EntityTypeBuilder<ActasSite> builder)
        {
            builder.ToTable("ActasSites","Seguimiento");

        }
    }
}