using AutoMapper;
using NotificationService.Application.Notifications.Dtos;
using NotificationService.Domain;

namespace NotificationService.Application.Notifications.Mappings;

public class NotificationMappings : Profile
{
    public NotificationMappings()
    {
        CreateMap<Notification, NotificationItemDto>();
        CreateMap<CreateNotificationDto, Notification>();
    }
}


