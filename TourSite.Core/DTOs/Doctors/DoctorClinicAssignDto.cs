using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorClinicAssignDto
    {
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int ClinicId { get; set; }
        public bool IsPrimaryClinic { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}
