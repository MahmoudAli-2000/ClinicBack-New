using AutoMapper;
using TourSite.Core.DTOs.Specialties;
using TourSite.Core.Entities;


namespace TourSite.Core.Mapping
{
    public class SpecialtyProfile : Profile
    {
        public SpecialtyProfile()
        {
            CreateMap<Specialty, SpecialtyDto>();
            CreateMap<SpecialtyCreateDto, Specialty>()
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
            CreateMap<SpecialtyUpdateDto, Specialty>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
        }
    }
}