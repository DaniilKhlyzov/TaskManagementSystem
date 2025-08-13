namespace NotificationService.Application.Notifications.Dtos;

public record NotificationItemDto(Guid Id, Guid UserId, string Title, string? Message, bool IsRead, DateTime CreatedAtUtc);
public record CreateNotificationDto(Guid UserId, string Title, string? Message);


