using AutoMapper;
using TourSite.Core.DTOs.Visits;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class VitalSignProfile : Profile
    {
        public VitalSignProfile()
        {
           CreateMap<VitalSign, VitalSignDto>();
           CreateMap<VitalSignCreateDto, VitalSign>()
                .ForMember(dest => dest.RecordedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}