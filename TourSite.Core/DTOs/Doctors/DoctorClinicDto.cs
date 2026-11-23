
namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorClinicDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public bool IsPrimaryClinic { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool IsActive { get; set; }
    }
}
