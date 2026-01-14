using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Journaway.IntegrationTests;

public static class DbMigration
{
    public static async Task MigrateAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<OccupancyDbContext>();
        await db.Database.MigrateAsync();
    }
}