
namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorDetailsDto : DoctorListDto
    {
        public decimal? DefaultConsultationFee { get; set; }
        public string? Bio { get; set; }
        public string? UserId { get; set; }
        public IReadOnlyList<DoctorClinicDto> Clinics { get; set; } = Array.Empty<DoctorClinicDto>();
    }
}
