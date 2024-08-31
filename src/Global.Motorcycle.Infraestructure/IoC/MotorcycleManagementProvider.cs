using Global.Motorcycle.Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Global.Motorcycle.Application.Features.Motorcycles.Commands.UpdateMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycle;
using Global.Motorcycle.Application.Features.Motorycycles.Queries.GetMotorcycleById;
using Global.Motorcycle.Domain.Contracts.Cache;
using Global.Motorcycle.Domain.Contracts.Data;
using Global.Motorcycle.Domain.Contracts.Data.Repositories;
using Global.Motorcycle.Domain.Contracts.Date;
using Global.Motorcycle.Domain.Contracts.Events;
using Global.Motorcycle.Domain.Contracts.ExternalServices;
using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Infraestructure.Cache;
using Global.Motorcycle.Infraestructure.Data;
using Global.Motorcycle.Infraestructure.Data.Repositories;
using Global.Motorcycle.Infraestructure.Date;
using Global.Motorcycle.Infraestructure.Events.Locations;
using Global.Motorcycle.Infraestructure.Events.Motorcycles;
using Global.Motorcycle.Infraestructure.ExternalServices;
using Global.Motorcycle.Infraestructure.Validation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Global.Motorcycle.Infraestructure.IoC
{
    public static class MotorcycleManagementProvider
    {
        public static IServiceCollection AddProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

            var connectionString = configuration.GetConnectionString("MotorcycleManagement");


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastValidator<,>));
            services.AddScoped<INotificationsHandler, NotificationHandler>();
            services.AddDbContextPool<MotorcycleManagementContext>(opt => opt.UseNpgsql(connectionString));
            services.AddTransient<IMotorcycleRepository, MotorcycleRepository>();
            services.AddTransient<IMotorcycleProducer, MotorcycleProducer>();
            services.AddTransient<IMotorcycleCache, MotorcycleCache>();
            services.AddTransient<ILocationProducer, LocationProducer>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddAutoMapper(typeof(CreateMotorcycleMapper));
            services.AddAutoMapper(typeof(UpdateMotorcycleMapper));
            services.AddAutoMapper(typeof(GetMotorcycleByIdMapper));
            services.AddAutoMapper(typeof(GetMotorcycleMapper));
            services.AddTransient<ISystemDate, SystemDate>();
            services.AddHttpClient<IDeliveryExternalService, DeliveryExternalService>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(configuration.GetSection("Delivery:Uri").Value);
            });

            return services;
        }
    }
}
