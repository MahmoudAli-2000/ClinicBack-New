using TourSite.Core.DTOs.Visits;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IVisitService
    {
        Task<VisitDto> StartFromAppointmentAsync(int appointmentId, VisitStartDto dto, CancellationToken cancellationToken = default);
        Task<VisitDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<VisitDto> CompleteAsync(int id, VisitUpdateDto dto, CancellationToken cancellationToken = default);
        Task<VisitDto> UpdateNotesAsync(int id, VisitUpdateDto dto, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<VisitDto>> GetPatientHistoryAsync(int patientId, CancellationToken cancellationToken = default);
        Task<VitalSignDto> AddVitalAsync(int visitId, VitalSignCreateDto dto, CancellationToken cancellationToken = default);
        Task<VitalSignDto> UpdateVitalAsync(int vitalId, VitalSignCreateDto dto, CancellationToken cancellationToken = default);
        Task DeleteVitalAsync(int vitalId, CancellationToken cancellationToken = default);
        Task<DiagnosisDto> AddDiagnosisAsync(int visitId, DiagnosisCreateDto dto, CancellationToken cancellationToken = default);
        Task<DiagnosisDto> UpdateDiagnosisAsync(int diagnosisId, DiagnosisCreateDto dto, CancellationToken cancellationToken = default);
        Task DeleteDiagnosisAsync(int diagnosisId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DiagnosisDto>> GetPatientDiagnosesAsync(int patientId, CancellationToken cancellationToken = default);
    }
}
