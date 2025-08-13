using Microsoft.AspNetCore.SignalR;

namespace NotificationService.API.Services;

public sealed class SignalRNotificationSender : NotificationService.Application.Notifications.Commands.INotificationSender
{
    private readonly IHubContext<NotificationHub> _hub;
    public SignalRNotificationSender(IHubContext<NotificationHub> hub) { _hub = hub; }

    public async Task SendToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
    {
        await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", payload, cancellationToken);
    }
}


