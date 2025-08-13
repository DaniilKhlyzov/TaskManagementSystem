namespace TaskService.Domain;

/// <summary>
/// Статус задачи (job) в системе
/// </summary>
public enum JobStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Archived = 3
}

/// <summary>
/// Доменная модель задачи (job)
/// </summary>
public class Job
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
        = null;
    public JobStatus Status { get; set; } = JobStatus.Pending;
    public Guid? AssigneeId { get; set; }
        = null;
    public bool IsDeleted { get; set; }
        = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
        = null;
}

public enum JobAction
{
    Created = 0,
    Updated = 1,
    Assigned = 2,
    Deleted = 3
}

/// <summary>
/// История изменений по задаче
/// </summary>
public class JobHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid JobId { get; set; }
        = Guid.Empty;
    public JobAction Action { get; set; }
        = JobAction.Created;
    public Guid? ActorUserId { get; set; }
        = null;
    public string? Comment { get; set; }
        = null;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
