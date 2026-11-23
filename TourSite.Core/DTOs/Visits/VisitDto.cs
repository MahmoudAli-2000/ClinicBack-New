using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Visits
{
    public class VisitDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public DateTime? CheckInAtUtc { get; set; }
        public DateTime? VisitStartAtUtc { get; set; }
        public DateTime? VisitEndAtUtc { get; set; }
        public string? ChiefComplaint { get; set; }
        public string? DoctorNotes { get; set; }
        public VisitStatus Status { get; set; }
        public IReadOnlyList<VitalSignDto> VitalSigns { get; set; } = Array.Empty<VitalSignDto>();
        public IReadOnlyList<DiagnosisDto> Diagnoses { get; set; } = Array.Empty<DiagnosisDto>();
    }
}
