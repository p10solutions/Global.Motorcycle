using Global.Motorcycle.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Global.Motorcycle.Api.Configuration
{
    public static class DataBaseConfig
    {
        public static void RunMigrations(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var context = services.GetRequiredService<MotorcycleManagementContext>();
                context.Database.Migrate();
            }
        }
    }
}
