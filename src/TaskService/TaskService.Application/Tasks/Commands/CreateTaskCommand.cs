using AutoMapper;
using FluentValidation;
using MediatR;
using Common.Common.Responses;
using TaskService.Application.Tasks.Dtos;
using TaskService.Domain;
using TaskService.Infrastructure;

namespace TaskService.Application.Tasks.Commands;

public record CreateTaskCommand(CreateTaskDto Dto) : IRequest<TaskDetailsDto>;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Description).MaximumLength(2000);
    }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDetailsDto>
{
    private readonly JobsDbContext _db;
    private readonly IMapper _mapper;

    public CreateTaskCommandHandler(JobsDbContext db, IMapper mapper)
    { _db = db; _mapper = mapper; }

    public async Task<TaskDetailsDto> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<Job>(request.Dto);
        await _db.Jobs.AddAsync(entity, ct);
        await _db.Histories.AddAsync(new JobHistory { JobId = entity.Id, Action = JobAction.Created }, ct);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<TaskDetailsDto>(entity);
    }
}


