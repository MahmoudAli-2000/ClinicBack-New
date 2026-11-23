
namespace TourSite.Core.DTOs.UserAuth
{
    public class CurrentUserDto
    {
        public string ImageCover { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
        public int? PatientId { get; set; }
        public int? StaffId { get; set; }
        public int? DoctorId { get; set; }
    }
}
