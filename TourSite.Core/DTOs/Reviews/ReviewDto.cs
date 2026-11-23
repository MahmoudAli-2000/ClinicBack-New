
namespace TourSite.Core.DTOs.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public byte? DoctorRating { get; set; }
        public byte? ClinicRating { get; set; }
        public byte? StaffRating { get; set; }
        public string? Comment { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime ReviewedAtUtc { get; set; }
    }
}
