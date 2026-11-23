using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class TimeOffProfile : Profile
    {
        public TimeOffProfile()
        {
           /maps<DoctorTimeOff, TimeOffDto>();
           /maps<TimeOffCreateDto, DoctorTimeOff>()
                .ForMember(dest => dest.Status, opt => opt MAPFrom(_ => TimeOffStatus.Approved))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}