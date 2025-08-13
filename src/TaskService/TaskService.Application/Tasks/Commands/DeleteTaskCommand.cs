using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Infrastructure;

namespace TaskService.Application.Tasks.Commands;

public record DeleteTaskCommand(Guid Id) : IRequest<Unit>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
{
    private readonly JobsDbContext _db;
    public DeleteTaskCommandHandler(JobsDbContext db) { _db = db; }

    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var task = await _db.Jobs.FirstOrDefaultAsync(t => t.Id == request.Id, ct) ?? throw new KeyNotFoundException();
        task.IsDeleted = true;
        await _db.Histories.AddAsync(new Domain.JobHistory { JobId = task.Id, Action = Domain.JobAction.Deleted }, ct);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}


