using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class DiagnosisProfile : Profile
    {
        public DiagnosisProfile()
        {
           /maps<Diagnosis, DiagnosisDto>();
           /maps<DiagnosisCreateDto, Diagnosis>()
                .ForMember(dest => dest.DiagnosedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}