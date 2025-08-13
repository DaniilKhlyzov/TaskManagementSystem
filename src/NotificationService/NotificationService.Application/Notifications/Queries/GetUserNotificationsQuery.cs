using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Notifications.Dtos;
using NotificationService.Infrastructure;

namespace NotificationService.Application.Notifications.Queries;

public record GetUserNotificationsQuery(Guid UserId) : IRequest<IReadOnlyCollection<NotificationItemDto>>;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, IReadOnlyCollection<NotificationItemDto>>
{
    private readonly NotificationDbContext _db;
    private readonly IMapper _mapper;
    public GetUserNotificationsQueryHandler(NotificationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<IReadOnlyCollection<NotificationItemDto>> Handle(GetUserNotificationsQuery request, CancellationToken ct)
    {
        return await _db.Notifications.AsNoTracking()
            .Where(n => n.UserId == request.UserId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ProjectTo<NotificationItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}


