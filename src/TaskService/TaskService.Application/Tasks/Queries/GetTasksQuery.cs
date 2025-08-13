using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Common.Common.Responses;
using TaskService.Application.Tasks.Dtos;
using TaskService.Infrastructure;
using TaskService.Domain;

namespace TaskService.Application.Tasks.Queries;

public record GetTasksQuery(int Page = 1, int PageSize = 20, JobStatus? Status = null, Guid? AssigneeId = null, string? Search = null) : IRequest<PagedResponse<TaskListItemDto>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedResponse<TaskListItemDto>>
{
    private readonly JobsDbContext _db;
    private readonly IMapper _mapper;

    public GetTasksQueryHandler(JobsDbContext db, IMapper mapper)
    {
        _db = db; _mapper = mapper;
    }

    public async Task<PagedResponse<TaskListItemDto>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        IQueryable<Job> query = _db.Jobs.AsNoTracking();
        if (request.Status.HasValue) query = query.Where(t => t.Status == request.Status);
        if (request.AssigneeId.HasValue) query = query.Where(t => t.AssigneeId == request.AssigneeId);
        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(t => t.Title.Contains(request.Search) || (t.Description != null && t.Description.Contains(request.Search)));

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<TaskListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return new PagedResponse<TaskListItemDto>(items, total, request.Page, request.PageSize);
    }
}


