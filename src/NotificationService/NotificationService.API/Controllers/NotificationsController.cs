using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Notifications.Commands;
using NotificationService.Infrastructure;
using NotificationService.Domain;

namespace NotificationService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _db;
    private readonly INotificationSender _sender;
    public NotificationsController(NotificationDbContext db, INotificationSender sender) { _db = db; _sender = sender; }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> List(Guid userId)
    {
        var items = await _db.Notifications.AsNoTracking().Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAtUtc).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Notification input)
    {
		if (string.IsNullOrWhiteSpace(input.Title))
		{
			return BadRequest("Title is required");
		}
		var notif = new Notification { UserId = input.UserId, Title = input.Title, Message = input.Message };
        await _db.Notifications.AddAsync(notif);
        await _db.SaveChangesAsync();
        await _sender.SendToUserAsync(notif.UserId, notif);
		return CreatedAtAction(nameof(List), new { userId = notif.UserId }, notif);
    }
}


