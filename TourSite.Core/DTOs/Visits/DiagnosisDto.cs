using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Visits
{
    public class DiagnosisDto
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public string? DiagnosisCode { get; set; }
        public string DiagnosisName { get; set; } = string.Empty;
        public DiagnosisType DiagnosisType { get; set; }
        public string? Notes { get; set; }
        public int? DiagnosedByDoctorId { get; set; }
        public DateTime DiagnosedAtUtc { get; set; }
    }
}
