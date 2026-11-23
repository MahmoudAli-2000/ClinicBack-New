using Automapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core DTOs.Clinics;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class ClinicProfile : Profile
    {
        public ClinicProfile(IConfiguration configuration)
        {
           CreateMap<Clinic, ClinicDto>()
                 .ForMember(d => d.ImageCover, o => o.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"));
           CreateMap<ClinicCreateDto, Clinic>()
                 .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
           CreateMap<ClinicUpdateDto, Clinic>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
        }
    }
}
