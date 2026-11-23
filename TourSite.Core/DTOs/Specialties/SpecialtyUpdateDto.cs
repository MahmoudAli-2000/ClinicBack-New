
namespace TourSite.Core.DTOs.Specialties
{
    public class SpecialtyUpdateDto : SpecialtyCreateDto
    {
        public bool IsActive { get; set; } = true;
    }
}
