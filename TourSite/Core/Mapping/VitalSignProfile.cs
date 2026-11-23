using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class VitalSignProfile : Profile
    {
        public VitalSignProfile()
        {
           /maps<VitalSign, VitalSignDto>();
           /maps<VitalSignCreateDto, VitalSign>()
                .ForMember(dest => dest.RecordedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow));
        }
    }
}