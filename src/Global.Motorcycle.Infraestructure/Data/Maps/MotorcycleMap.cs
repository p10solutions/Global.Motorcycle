using Global.Motorcycle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Global.Motorcycle.Infraestructure.Data.Maps
{
    public class MotorcycleMap : IEntityTypeConfiguration<MotorcycleEntity>
    {
        public void Configure(EntityTypeBuilder<MotorcycleEntity> builder)
        {
            builder.ToTable("TB_MOTORCYCLE");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.Model)
                .HasColumnName("MODEL")
                .HasColumnType("varchar(200)");

            builder.Property(x => x.Plate)
                .HasColumnName("PLATE")
                .HasColumnType("varchar(10)");

            builder.Property(x => x.CreateDate)
                .HasColumnName("DT_CREATE")
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.UpdateDate)
                .HasColumnName("DT_UPDATE")
                .HasColumnType("timestamp without time zone");

            builder.Property(x => x.Status)
                .HasColumnName("STATUS");
        }
    }
}
