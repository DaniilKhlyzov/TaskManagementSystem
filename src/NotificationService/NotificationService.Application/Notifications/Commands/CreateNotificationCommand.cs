using AutoMapper;
using FluentValidation;
using MediatR;
using NotificationService.Application.Notifications.Dtos;
using NotificationService.Domain;
using NotificationService.Infrastructure;

namespace NotificationService.Application.Notifications.Commands;

public record CreateNotificationCommand(CreateNotificationDto Dto) : IRequest<NotificationItemDto>;

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.Dto.UserId).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Message).MaximumLength(2000);
    }
}

public interface INotificationSender
{
    Task SendToUserAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationItemDto>
{
    private readonly NotificationDbContext _db;
    private readonly IMapper _mapper;
    private readonly INotificationSender _sender;

    public CreateNotificationCommandHandler(NotificationDbContext db, IMapper mapper, INotificationSender sender)
    { _db = db; _mapper = mapper; _sender = sender; }

    public async Task<NotificationItemDto> Handle(CreateNotificationCommand request, CancellationToken ct)
    {
        var entity = _mapper.Map<Notification>(request.Dto);
        await _db.Notifications.AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        await _sender.SendToUserAsync(entity.UserId, entity, ct);
        return _mapper.Map<NotificationItemDto>(entity);
    }
}


