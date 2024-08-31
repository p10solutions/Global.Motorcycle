using Global.Motorcycle.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Global.Motorcycle.Infraestructure.Data
{
    public class MotorcycleManagementContext(DbContextOptions<MotorcycleManagementContext> options) : DbContext(options)
    {
        public DbSet<MotorcycleEntity> Motorcycle { get; set; }
        public DbSet<Plan> Plan { get; set; }
        public DbSet<Location> Location { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
