using Microsoft.AspNetCore.Mvc;

namespace Journaway.WebApi.AppStart;

public static class Dependencies
{
    public static IServiceCollection AddWebApi(this IServiceCollection services)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Title = "Request validation failed.",
                        Status = StatusCodes.Status400BadRequest,
                        Type = "https://httpstatuses.com/400"
                    };

                    return new BadRequestObjectResult(problemDetails);
                };
            });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        return services;
    }
}