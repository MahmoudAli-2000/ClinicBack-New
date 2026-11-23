using TourSite.Core.DTOs.Specialties;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface ISpecialtyService
    {
        Task<SpecialtyDto> CreateAsync(SpecialtyCreateDto dto, CancellationToken cancellationToken = default);
        Task<SpecialtyDto> UpdateAsync(int id, SpecialtyUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<SpecialtyDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<SpecialtyDto>> SearchAsync(PagedQueryDto query, CancellationToken cancellationToken = default);
    }
}
