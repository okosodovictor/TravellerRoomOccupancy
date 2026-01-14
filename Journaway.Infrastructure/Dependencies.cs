using Journaway.Application.Occupancy;
using Journaway.Application.Repositories;
using Journaway.Infrastructure.Occupancy;
using Journaway.Infrastructure.Persistence;
using Journaway.Infrastructure.Repositories;
using Journaway.WebApi.DevelomentSeeder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Journaway.Infrastructure;


public static class Dependencies
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OccupancyDbContext>(options =>
        {
            options.UseNpgsql(
                config.GetConnectionString("OccupancyDb"),
                npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(Dependencies).Assembly.FullName);
                });
        });
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Repositories
        services.AddScoped<IRoomRepository, EfRoomRepository>();
        services.AddScoped<ITravelGroupRepository, EfTravelGroupRepository>();
        services.AddScoped<ITravellerRepository, EfTravellerRepository>();
        services.AddScoped<IRoomAssignmentRepository, EfRoomAssignmentRepository>();

        // Read-side queries
        services.AddScoped<IOccupancyQueries, EfOccupancyQueries>();
        
        services.AddScoped<DevDataSeeder>();
        
        return services;
    }
}