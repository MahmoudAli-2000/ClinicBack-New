using AutoMapper;
using TourSite.Core.DTOs.Notifications;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
           CreateMap<Notification, NotificationDto>();
           CreateMap<NotificationPreference, NotificationPreferenceDto>().ReverseMap();
        }
    }
}