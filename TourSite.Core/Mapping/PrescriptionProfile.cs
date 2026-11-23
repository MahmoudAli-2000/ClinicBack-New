
using AutoMapper;
using TourSite.Core.DTOs.Prescriptions;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class PrescriptionProfile : Profile
    {
        public PrescriptionProfile()
        {
           CreateMap<Prescription, PrescriptionDto>();
           CreateMap<PrescriptionCreateDto, Prescription>()
                .ForMember(dest => dest.IssuedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
           CreateMap<PrescriptionItemCreateDto, PrescriptionItem>();
           CreateMap<PrescriptionItem, PrescriptionItemDto>()
                .ForMember(d => d.MedicationName, o => o.MapFrom(s => s.Medication.GenericName));
        }
    }
}