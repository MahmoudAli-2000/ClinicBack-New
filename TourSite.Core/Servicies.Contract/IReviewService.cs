using TourSite.Core.DTOs.Reviews;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateAsync(ReviewCreateDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReviewDto>> GetDoctorReviewsAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReviewDto>> GetClinicReviewsAsync(int clinicId, CancellationToken cancellationToken = default);
        Task<ReviewStatsDto> GetDoctorStatsAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<ReviewStatsDto> GetClinicStatsAsync(int clinicId, CancellationToken cancellationToken = default);
    }
}
