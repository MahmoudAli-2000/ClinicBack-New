using TourSite.Core.DTOs.Common;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Appointments
{
    public class AppointmentQueryDto : PagedQueryDto
    {
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public int? ClinicId { get; set; }
        public AppointmentStatus? Status { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
    }
}
