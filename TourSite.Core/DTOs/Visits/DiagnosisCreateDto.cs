using System.ComponentModel.DataAnnotations;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Visits
{
    public class DiagnosisCreateDto
    {
        [MaxLength(30)]
        public string? DiagnosisCode { get; set; }
        [Required, MaxLength(250)]
        public string DiagnosisName { get; set; } = string.Empty;
        public DiagnosisType DiagnosisType { get; set; } = DiagnosisType.Primary;
        [MaxLength(1500)]
        public string? Notes { get; set; }
    }
}
