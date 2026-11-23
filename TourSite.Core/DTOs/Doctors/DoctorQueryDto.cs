using TourSite.Core.DTOs.Common;

namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorQueryDto : PagedQueryDto
    {
        public int? SpecialtyId { get; set; }
        public int? ClinicId { get; set; }
        public bool? IsActive { get; set; }
    }
}
