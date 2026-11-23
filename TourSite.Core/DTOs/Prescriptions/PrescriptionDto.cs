using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Prescriptions
{
    public class PrescriptionDto
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int PrescribedByDoctorId { get; set; }
        public PrescriptionStatus Status { get; set; }
        public DateTime IssuedAtUtc { get; set; }
        public string? Notes { get; set; }
        public IReadOnlyList<PrescriptionItemDto> Items { get; set; } = Array.Empty<PrescriptionItemDto>();
    }
}
