using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class VisitProfile : Profile
    {
        public VisitProfile()
        {
           /maps<Visit, VisitDto>()
                .ForMember(d => d.VitalSigns, o => o MAPFrom(s => s.VitalSigns))
                .ForMember(d => d.Diagnoses, o => o MAPFrom(s => s.Diagnoses));
        }
    }
}