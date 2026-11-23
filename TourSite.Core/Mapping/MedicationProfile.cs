using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core.DTOs.Medications;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class MedicationProfile : Profile
    {
        public MedicationProfile(IConfiguration configuration)
        {
           CreateMap<Medication, MedicationDto>();
           CreateMap<MedicationCreateDto, Medication>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty));
            CreateMap<MedicationUpdateDto, Medication>()
                 .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                   .ForMember(d => d.ImageCover,
                     opt => opt.MapFrom((src, dest, _, ctx) =>
                         $"{configuration["BaseUrl"]}{src.ImageCover}"));
        }
    }
}