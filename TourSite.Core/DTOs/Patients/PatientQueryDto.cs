using TourSite.Core.DTOs.Common;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Patients
{
    public class PatientQueryDto : PagedQueryDto
    {
        public PatientStatus? Status { get; set; }
    }
}
