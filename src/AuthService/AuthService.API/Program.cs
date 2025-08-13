using Microsoft.EntityFrameworkCore;
using AuthService.Infrastructure.Data;
using AuthService.Domain;
using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Infrastructure.Repositories;
using Common.Common.Health;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AuthDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs)) options.UseInMemoryDatabase("auth-db");
    else options.UseNpgsql(cs);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<AuthAppService>();
builder.Services.AddCommonHealthChecks();

var app = builder.Build();


app.UseRouting();
app.MapHealthChecks("/health");

// Minimal API endpoints
app.MapPost("/api/auth/register", async (AuthAppService authService, RegisterRequestDto request) =>
{
    try
    {
        var result = await authService.RegisterAsync(request);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/auth/login", async (AuthAppService authService, LoginRequestDto request) =>
{
    try
    {
        var result = await authService.LoginAsync(request);
        return Results.Ok(result);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Unauthorized();
    }
});

using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
    var defaultCs = config.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(defaultCs))
    {
        var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await context.Database.MigrateAsync();
    }
}

app.Run();
