using Journaway.Application;
using Journaway.Infrastructure;
using Journaway.Infrastructure.Persistence;
using Journaway.WebApi.AppStart;
using Journaway.WebApi.DevelomentSeeder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.

builder.Services.AddWebApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DevDataSeeder>();
    
    var db = scope.ServiceProvider.GetRequiredService<OccupancyDbContext>();
    await db.Database.MigrateAsync();
    
    await seeder.SeedAsync(CancellationToken.None);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }