using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure;
using Common.Common.Health;
using Common.Common.Error;
using NotificationService.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NotificationDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(cs)) options.UseInMemoryDatabase("notifications-db");
    else options.UseNpgsql(cs);
});

builder.Services.AddSignalR();
builder.Services.AddCommonHealthChecks();
builder.Services.AddControllers();
builder.Services.AddSingleton<NotificationService.Application.Notifications.Commands.INotificationSender, SignalRNotificationSender>();

var app = builder.Build();

app.UseRouting();
app.UseGlobalErrorHandling();
app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

public class NotificationHub : Hub { }

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Используем NameIdentifier из JWT как UserId SignalR
        return connection.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
