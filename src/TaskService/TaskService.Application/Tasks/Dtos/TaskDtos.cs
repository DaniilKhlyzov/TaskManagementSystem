using TaskService.Domain;

namespace TaskService.Application.Tasks.Dtos;

public record TaskListItemDto(Guid Id, string Title, string? Description, JobStatus Status, Guid? AssigneeId, DateTime CreatedAtUtc);
public record TaskDetailsDto(Guid Id, string Title, string? Description, JobStatus Status, Guid? AssigneeId, DateTime CreatedAtUtc, DateTime? UpdatedAtUtc);
public record CreateTaskDto(string Title, string? Description, JobStatus Status);
public record UpdateTaskDto(string Title, string? Description, JobStatus Status);


