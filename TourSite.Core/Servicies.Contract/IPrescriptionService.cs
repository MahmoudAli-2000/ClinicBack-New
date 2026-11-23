using TourSite.Core.DTOs.Prescriptions;
using TourSite.Core.Helper;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Servicies.Contract
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDto> CreateAsync(PrescriptionCreateDto dto, CancellationToken cancellationToken = default);
        Task<PrescriptionDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrescriptionDto>> GetPatientHistoryAsync(int patientId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<PrescriptionDto>> GetDoctorHistoryAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<PrescriptionDto> UpdateStatusAsync(int id, PrescriptionStatus status, CancellationToken cancellationToken = default);
    }
}
