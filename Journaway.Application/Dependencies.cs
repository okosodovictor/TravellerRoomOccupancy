using Journaway.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Journaway.Application;

public static class Dependencies
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register application services later
        services.AddScoped<IMoveTravellerUseCase, MoveTravellerUseCase>();
        services.AddScoped<IAssignTravellerUseCase, AssignTravellerUseCase>();
        return services;
    }
}