using TourSite.Core.DTOs.Appointments;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default);
        Task<AppointmentDto> RescheduleAsync(int id, AppointmentRescheduleDto dto, CancellationToken cancellationToken = default);
        Task<AppointmentDto> CancelAsync(int id, AppointmentCancelDto dto, CancellationToken cancellationToken = default);
        Task<AppointmentDto> ConfirmAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> CheckInAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> CompleteAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> MarkNoShowAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PageinationResponse<AppointmentDto>> SearchAsync(AppointmentQueryDto query, CancellationToken cancellationToken = default);
    }
}
