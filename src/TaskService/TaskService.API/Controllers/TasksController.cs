using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskService.Domain;
using TaskService.Infrastructure;

namespace TaskService.API.Controllers;

[ApiController]
[Authorize]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly JobsDbContext _db;

    public TasksController(JobsDbContext db)
    { _db = db; }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] JobStatus? status = null, [FromQuery] Guid? assigneeId = null, [FromQuery] string? search = null)
    {
        IQueryable<Job> query = _db.Jobs.AsNoTracking();
        if (status.HasValue) query = query.Where(t => t.Status == status);
        if (assigneeId.HasValue) query = query.Where(t => t.AssigneeId == assigneeId);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search) || (t.Description != null && t.Description.Contains(search)));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _db.Jobs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (task is null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Job input)
    {
        if (string.IsNullOrWhiteSpace(input.Title)) return BadRequest("Title is required");
        var task = new Job { Title = input.Title, Description = input.Description, Status = input.Status };
        await _db.Jobs.AddAsync(task);
        await _db.Histories.AddAsync(new JobHistory { JobId = task.Id, Action = JobAction.Created });
        await _db.SaveChangesAsync();
        return Created($"/api/tasks/{task.Id}", task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Job input)
    {
        var task = await _db.Jobs.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return NotFound();
        task.Title = input.Title;
        task.Description = input.Description;
        task.Status = input.Status;
        task.UpdatedAtUtc = DateTime.UtcNow;
        await _db.Histories.AddAsync(new JobHistory { JobId = task.Id, Action = JobAction.Updated });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _db.Jobs.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return NotFound();
        task.IsDeleted = true;
        await _db.Histories.AddAsync(new JobHistory { JobId = task.Id, Action = JobAction.Deleted });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id:guid}/assign")]
    public async Task<IActionResult> Assign(Guid id, [FromQuery] Guid assigneeId)
    {
        var task = await _db.Jobs.FirstOrDefaultAsync(t => t.Id == id);
        if (task is null) return NotFound();
        task.AssigneeId = assigneeId;
        task.UpdatedAtUtc = DateTime.UtcNow;
        await _db.Histories.AddAsync(new JobHistory { JobId = task.Id, Action = JobAction.Assigned, ActorUserId = assigneeId });
        await _db.SaveChangesAsync();
        return NoContent();
    }
}


