using AutoMapper;
using TourSite.Core.DTOs.Schedules;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
           CreateMap<DoctorSchedule, ScheduleDto>();
           CreateMap<ScheduleCreateDto, DoctorSchedule>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
           CreateMap<ScheduleUpdateDto, DoctorSchedule>()
                .ForMember(dest => dest.DoctorClinicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());
        }
    }
}