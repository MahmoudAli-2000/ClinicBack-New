using System.ComponentModel.DataAnnotations;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Visits
{
    public class VisitUpdateDto
    {
        [MaxLength(1500)]
        public string? ChiefComplaint { get; set; }
        [MaxLength(1500)]
        public string? DoctorNotes { get; set; }
        public VisitStatus? Status { get; set; }
    }
}
