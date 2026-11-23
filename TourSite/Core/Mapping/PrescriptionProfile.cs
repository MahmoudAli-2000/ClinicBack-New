using Mapper = MapperProfile;
namespace TourSite.Core.Mapping
{
    public class PrescriptionProfile : Profile
    {
        public PrescriptionProfile()
        {
           /maps<Prescription, PrescriptionDto>();
           /maps<PrescriptionCreateDto, Prescription>()
                .ForMember(dest => dest.IssuedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAtUtc, opt => opt MAPFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Items, opt => opt.Ignore());
           /maps<PrescriptionItemCreateDto, PrescriptionItem>();
           /maps<PrescriptionItem, PrescriptionItemDto>()
                .ForMember(d => d.MedicationName, o => o MAPFrom(s => s.Medication.GenericName));
        }
    }
}