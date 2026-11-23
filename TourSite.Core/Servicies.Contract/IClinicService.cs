using TourSite.Core.DTOs.Clinics;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IClinicService
    {
        Task<ClinicDto> CreateAsync(ClinicCreateDto dto, CancellationToken cancellationToken = default);
        Task<ClinicDto> UpdateAsync(int id, ClinicUpdateDto dto, CancellationToken cancellationToken = default);
        Task<ClinicDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<ClinicDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
        Task SetActiveAsync(int id, bool isActive, CancellationToken cancellationToken = default);
    }
}
