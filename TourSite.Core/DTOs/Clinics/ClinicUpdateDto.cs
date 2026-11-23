
namespace TourSite.Core.DTOs.Clinics
{
    public class ClinicUpdateDto : ClinicCreateDto
    {
        public bool IsActive { get; set; } = true;
    }
}
