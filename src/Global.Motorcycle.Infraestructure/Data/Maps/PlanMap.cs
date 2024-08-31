using Global.Motorcycle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Global.Motorcycle.Infraestructure.Data.Maps
{
    public class PlanMap : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.ToTable("TB_PLAN");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.Name)
                .HasColumnName("NAME")
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Days)
                .HasColumnName("DAYS");

            builder.Property(x => x.Daily)
                .HasColumnName("DAILY");

            builder.Property(x => x.FeeBefore)
                .HasColumnName("FEE_BEFORE")
                .HasColumnType("numeric(10,2)");

            builder.Property(x => x.FeeAfter)
                .HasColumnName("FEE_AFTER")
                .HasColumnType("numeric(10,2)");
        }
    }
}
