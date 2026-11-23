using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class DoctorClinicProfile : Profile
    {
        public DoctorClinicProfile()
        {
           /maps<DoctorClinic, DoctorClinicDto>()
                .ForMember(d => d.ClinicName, o => o MAPFrom(s => s.Clinic.ClinicName));
           /maps<DoctorClinicAssignDto, DoctorClinic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.JoinedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt MAPFrom(_ => true));
        }
    }
}