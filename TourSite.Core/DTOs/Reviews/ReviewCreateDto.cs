using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Reviews
{
    public class ReviewCreateDto
    {
        [Required]
        public int AppointmentId { get; set; }
        [Range(1, 5)]
        public byte? DoctorRating { get; set; }
        [Range(1, 5)]
        public byte? ClinicRating { get; set; }
        [Range(1, 5)]
        public byte? StaffRating { get; set; }
        [MaxLength(2000)]
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
