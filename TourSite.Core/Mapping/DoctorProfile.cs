using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core.DTOs.Doctors;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile(IConfiguration configuration)
        {
            CreateMap<Doctor, DoctorListDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Staff.FirstName + " " + s.Staff.LastName))
                .ForMember(d => d.SpecialtyName, o => o.MapFrom(s => s.Specialty.SpecialtyName))
                .ForMember(d => d.ImageCover, o => o.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"));
            CreateMap<Doctor, DoctorDetailsDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Staff.FirstName + " " + s.Staff.LastName))
                .ForMember(d => d.SpecialtyName, o => o.MapFrom(s => s.Specialty.SpecialtyName))
                .ForMember(d => d.ImageCover, o => o.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"));
            CreateMap<DoctorCreateDto, Doctor>()
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty))
                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
            CreateMap<DoctorUpdateDto, Doctor>()
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Staff, opt => opt.Ignore())
                .ForMember(dest => dest.Specialty, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorClinics, opt => opt.Ignore())
                .ForMember(dest => dest.TimeOffs, opt => opt.Ignore())
                .ForMember(dest => dest.Diagnoses, opt => opt.Ignore())
                .ForMember(dest => dest.Prescriptions, opt => opt.Ignore())
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty))
                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
        }
    }
}