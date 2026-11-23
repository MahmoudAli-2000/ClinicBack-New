using AutoMapper;
using Microsoft.Extensions.Configuration;
using TourSite.Core.DTOs.Staff;
using TourSite.Core.Entities;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Mapping
{
    public class StaffProfile : Profile
    {
        public StaffProfile(IConfiguration configuration)
        {
           CreateMap<Staff, StaffDto>()
                .ForMember(d => d.DoctorId, o => o.MapFrom(s => s.Doctor != null ? s.Doctor.Id : (int?)null))
                .ForMember(d => d.ImageCover,
                     opt => opt.MapFrom((src, dest, _, ctx) =>
                         $"{configuration["BaseUrl"]}{src.ImageCover}"))
                .ForMember(d => d.Roles, o => o.Ignore());
           CreateMap<StaffCreateDto, Staff>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => StaffStatus.Active))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty))
                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
           CreateMap<StaffUpdateDto, Staff>()
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.ImageCover, opt => opt.MapFrom(src => src.ImageCover ?? string.Empty))
                .ForMember(dest => dest.PublicCode, opt => opt.Ignore());
        }
    }
}