using Microsoft.EntityFrameworkCore;
using TaskService.Infrastructure;
using TaskService.Domain;
using Common.Common.Error;
using Common.Common.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<JobsDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs))
    {
        options.UseInMemoryDatabase("tasks-db");
    }
    else
    {
        options.UseNpgsql(cs);
    }
});

builder.Services.AddLogging();
builder.Services.AddCommonHealthChecks();

var app = builder.Build();

// simplified pipeline

app.UseRouting();
app.UseGlobalErrorHandling();
app.MapHealthChecks("/health");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
    var defaultCs = config.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(defaultCs))
    {
        var context = scope.ServiceProvider.GetRequiredService<JobsDbContext>();
        await context.Database.MigrateAsync();
    }
}

app.Run();
