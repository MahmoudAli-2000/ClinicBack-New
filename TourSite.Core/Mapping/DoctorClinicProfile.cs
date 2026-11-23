using AutoMapper;
using TourSite.Core.DTOs.Doctors;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class DoctorClinicProfile : Profile
    {
        public DoctorClinicProfile()
        {
           CreateMap<DoctorClinic, DoctorClinicDto>()
                .ForMember(d => d.ClinicName, o => o.MapFrom(s => s.Clinic.ClinicName));
           CreateMap<DoctorClinicAssignDto, DoctorClinic>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.JoinedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
        }
    }
}