using TourSite.Core.DTOs.Patients;
using TourSite.Core.Helper;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Servicies.Contract
{
    public interface IPatientService
    {
        Task<PatientDetailsDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default);
        Task<PatientDetailsDto> UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken = default);
        Task<PatientDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<PatientListDto>> SearchAsync(PatientQueryDto query, CancellationToken cancellationToken = default);
        Task SetStatusAsync(int id, PatientStatus status, CancellationToken cancellationToken = default);
        Task EnsureCanAccessPatientAsync(int patientId, CancellationToken cancellationToken = default);
    }
}
