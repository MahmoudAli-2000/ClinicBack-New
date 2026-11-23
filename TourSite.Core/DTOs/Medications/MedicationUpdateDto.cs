
namespace TourSite.Core.DTOs.Medications
{
    public class MedicationUpdateDto : MedicationCreateDto
    {
        public bool IsActive { get; set; } = true;
    }
}
