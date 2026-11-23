using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Patients
{
    public class PatientDetailsDto : PatientListDto
    {
        public string? UserId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
