using Journaway.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Journaway.Infrastructure;

public sealed class OccupancyDbContextFactory : IDesignTimeDbContextFactory<OccupancyDbContext>
{
    public OccupancyDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connStr = configuration.GetConnectionString("OccupancyDb");
        if (string.IsNullOrWhiteSpace(connStr))
            throw new InvalidOperationException("Connection string 'OccupancyDb' not found.");

        var options = new DbContextOptionsBuilder<OccupancyDbContext>()
            .UseNpgsql(connStr)
            .Options;

        return new OccupancyDbContext(options);
    }
}