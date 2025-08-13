namespace NotificationService.Domain;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
        = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
        = null;
    public bool IsRead { get; set; }
        = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
