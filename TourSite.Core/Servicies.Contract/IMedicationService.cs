using TourSite.Core.DTOs.Medications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IMedicationService
    {
        Task<MedicationDto> CreateAsync(MedicationCreateDto dto, CancellationToken cancellationToken = default);
        Task<MedicationDto> UpdateAsync(int id, MedicationUpdateDto dto, CancellationToken cancellationToken = default);
        Task<MedicationDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<MedicationDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
        Task SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default);
    }
}
