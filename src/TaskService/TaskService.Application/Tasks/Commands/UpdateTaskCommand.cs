using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Tasks.Dtos;
using TaskService.Infrastructure;

namespace TaskService.Application.Tasks.Commands;

public record UpdateTaskCommand(Guid Id, UpdateTaskDto Dto) : IRequest<Unit>;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).MaximumLength(2000);
    }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Unit>
{
    private readonly JobsDbContext _db;
    private readonly IMapper _mapper;

    public UpdateTaskCommandHandler(JobsDbContext db, IMapper mapper)
    { _db = db; _mapper = mapper; }

    public async Task<Unit> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var task = await _db.Jobs.FirstOrDefaultAsync(t => t.Id == request.Id, ct) ?? throw new KeyNotFoundException();
        _mapper.Map(request.Dto, task);
        task.UpdatedAtUtc = DateTime.UtcNow;
        await _db.Histories.AddAsync(new Domain.JobHistory { JobId = task.Id, Action = Domain.JobAction.Updated }, ct);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}


