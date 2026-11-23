using TourSite.Core.DTOs.Staff;
using TourSite.Core.Helper;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Servicies.Contract
{
    public interface IStaffService
    {
        Task<StaffDto> CreateAsync(StaffCreateDto dto, CancellationToken cancellationToken = default);
        Task<StaffDto> UpdateAsync(int id, StaffUpdateDto dto, CancellationToken cancellationToken = default);
        Task<StaffDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<StaffDto>> SearchAsync(StaffQueryDto query, CancellationToken cancellationToken = default);
        Task SetStatusAsync(int id, StaffStatus status, CancellationToken cancellationToken = default);
    }
}
