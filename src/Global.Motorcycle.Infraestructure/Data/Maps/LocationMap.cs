using Global.Motorcycle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Global.Motorcycle.Infraestructure.Data.Maps
{
    public class LocationMap : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("TB_LOCATION");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.DeliverymanId)
                .HasColumnName("DELIVERYMAN_ID");

            builder.Property(x => x.MotorcycleId)
                .HasColumnName("MOTORCYCLE_ID");

            builder.Property(x => x.Amount)
                .HasColumnName("AMOUNT")
                .HasColumnType("numeric(10,2)");

            builder.Property(x => x.PlanId)
                .HasColumnName("PLAN_ID");

            builder.Property(x => x.InitialDate)
                .HasColumnName("DT_INITIAL")
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.EndDate)
                .HasColumnName("DT_END")
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.Paid)
                .HasColumnName("PAID");

            builder.Property(x => x.Fee)
                .HasColumnName("FEE")
                .HasColumnType("numeric(10,2)");

            builder.Property(x => x.DaysUse)
                .HasColumnName("DAYS_USE");

            builder.Property(x => x.Status)
                .HasColumnName("STATUS")
                .HasDefaultValue(ELocationStatus.Active);

            builder.HasOne(x => x.Plan)
                .WithMany()
                .HasForeignKey(x => x.PlanId)
                .IsRequired();

            builder.HasOne(x => x.Motorcycle)
                .WithMany()
                .HasForeignKey(x => x.MotorcycleId)
                .IsRequired();
        }
    }
}
