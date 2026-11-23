using TourSite.Core.DTOs.Common;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Staff
{
    public class StaffQueryDto : PagedQueryDto
    {
        public StaffStatus? Status { get; set; }
        public int? ClinicId { get; set; }
    }
}
