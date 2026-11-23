using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Visits
{
    public class VisitStartDto
    {
        [MaxLength(1500)]
        public string? ChiefComplaint { get; set; }
    }
}
