using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
           /maps<DoctorSchedule, ScheduleDto>();
           /maps<ScheduleCreateDto, DoctorSchedule>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow));
           /maps<ScheduleUpdateDto, DoctorSchedule>()
                .ForMember(dest => dest.DoctorClinicId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.Ignore());
        }
    }
}