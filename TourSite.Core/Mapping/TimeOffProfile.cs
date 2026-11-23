
using AutoMapper;
using TourSite.Core.DTOs.Schedules;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Mapping
{
    public class TimeOffProfile : Profile
    {
        public TimeOffProfile()
        {
           CreateMap<DoctorTimeOff, TimeOffDto>();
           CreateMap<TimeOffCreateDto, DoctorTimeOff>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TimeOffStatus.Approved))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}