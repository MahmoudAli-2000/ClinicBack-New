using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Appointments
{
    public class AppointmentCancelDto
    {
        [MaxLength(500)]
        public string? CancellationReason { get; set; }
    }
}
