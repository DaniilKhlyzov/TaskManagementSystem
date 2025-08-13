using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskService.Application.Tasks.Dtos;
using TaskService.Infrastructure;

namespace TaskService.Application.Tasks.Queries;

public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDetailsDto?>;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDetailsDto?>
{
    private readonly JobsDbContext _db;
    private readonly IMapper _mapper;

    public GetTaskByIdQueryHandler(JobsDbContext db, IMapper mapper)
    { _db = db; _mapper = mapper; }

    public async Task<TaskDetailsDto?> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
        return await _db.Jobs.AsNoTracking()
            .Where(t => t.Id == request.Id)
            .ProjectTo<TaskDetailsDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);
    }
}


