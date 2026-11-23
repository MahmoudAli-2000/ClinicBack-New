
namespace TourSite.Core.DTOs.Reviews
{
    public class ReviewStatsDto
    {
        public int Count { get; set; }
        public double? AverageDoctorRating { get; set; }
        public double? AverageClinicRating { get; set; }
        public double? AverageStaffRating { get; set; }
    }
}
