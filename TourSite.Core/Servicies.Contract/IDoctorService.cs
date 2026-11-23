using TourSite.Core.DTOs.Doctors;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IDoctorService
    {
        Task<DoctorDetailsDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default);
        Task<DoctorDetailsDto> UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken = default);
        Task<DoctorDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<DoctorListDto>> SearchAsync(DoctorQueryDto query, CancellationToken cancellationToken = default);
        Task<DoctorClinicDto> AssignClinicAsync(DoctorClinicAssignDto dto, CancellationToken cancellationToken = default);
        Task UnassignClinicAsync(int doctorClinicId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DoctorClinicDto>> GetDoctorClinicsAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<int?> GetDoctorIdForCurrentUserAsync(CancellationToken cancellationToken = default);
    }
}
