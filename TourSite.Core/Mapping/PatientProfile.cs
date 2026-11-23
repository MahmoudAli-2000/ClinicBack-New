using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core.DTOs.Patients;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;


namespace TourSite.Core.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile(IConfiguration configuration)
        {
           CreateMap<Patient, PatientListDto>();
           CreateMap<Patient, PatientDetailsDto>();
           CreateMap<PatientCreateDto, Patient>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => PatientStatus.Active))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty))
                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
           CreateMap<PatientUpdateDto, Patient>()
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(d => d.ImageCover,
                     opt => opt.MapFrom((src, dest, _, ctx) =>
                         $"{configuration["BaseUrl"]}{src.ImageCover}"))

                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
        }
    }
}