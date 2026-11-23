using AutoMapper;
using TourSite.Core.DTOs.Visits;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class VisitProfile : Profile
    {
        public VisitProfile()
        {
           CreateMap<Visit, VisitDto>()
                .ForMember(d => d.VitalSigns, o => o.MapFrom(s => s.VitalSigns))
                .ForMember(d => d.Diagnoses, o => o.MapFrom(s => s.Diagnoses));
        }
    }
}