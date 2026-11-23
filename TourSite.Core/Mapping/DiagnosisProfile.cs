using AutoMapper;
using TourSite.Core.DTOs.Visits;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class DiagnosisProfile : Profile
    {
        public DiagnosisProfile()
        {
           CreateMap<Diagnosis, DiagnosisDto>();
           CreateMap<DiagnosisCreateDto, Diagnosis>()
                .ForMember(dest => dest.DiagnosedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}