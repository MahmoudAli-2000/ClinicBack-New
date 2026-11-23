using Automapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core DTOs.Specialties;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class SpecialtyProfile : Profile
    {
        public SpecialtyProfile(IConfiguration configuration)
        {
           CreateMap<Specialty, SpecialtyDto>()
                 .ForMember(d => d.ImageCover, o => o.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"));
           CreateMap<SpecialtyCreateDto, Specialty>()
                 .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
           CreateMap<SpecialtyUpdateDto, Specialty>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
        }
    }
}


