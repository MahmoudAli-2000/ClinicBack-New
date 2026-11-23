using TourSite.Core.DTOs.Schedules;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IScheduleService
    {
        Task<ScheduleDto> CreateAsync(ScheduleCreateDto dto, CancellationToken cancellationToken = default);
        Task<ScheduleDto> UpdateAsync(int id, ScheduleUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ScheduleDto>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ScheduleDto>> GetByClinicAsync(int clinicId, CancellationToken cancellationToken = default);
        Task<TimeOffDto> CreateTimeOffAsync(TimeOffCreateDto dto, CancellationToken cancellationToken = default);
        Task DeleteTimeOffAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TimeOffDto>> GetTimeOffAsync(int doctorId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AvailabilitySlotDto>> GetAvailabilityAsync(AvailabilityQueryDto query, CancellationToken cancellationToken = default);
    }
}
