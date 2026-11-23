
namespace TourSite.Core.DTOs.Specialties
{
    public class SpecialtyDto
    {
        public int Id { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageCover { get; set; }
        public bool IsActive { get; set; }
    }
}
