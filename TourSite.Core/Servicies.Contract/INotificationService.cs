using TourSite.Core.DTOs.Notifications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;

namespace TourSite.Core.Servicies.Contract
{
    public interface INotificationService
    {
        Task PublishAsync(string userId, string type, string title, string message, int? appointmentId = null, CancellationToken cancellationToken = default);
        Task<PageinationResponse<NotificationDto>> GetMineAsync(bool unreadOnly, PagedQueryDto query, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);
        Task MarkReadAsync(int id, CancellationToken cancellationToken = default);
        Task MarkAllReadAsync(CancellationToken cancellationToken = default);
        Task<NotificationPreferenceDto> GetPreferencesAsync(CancellationToken cancellationToken = default);
        Task<NotificationPreferenceDto> UpdatePreferencesAsync(NotificationPreferenceDto dto, CancellationToken cancellationToken = default);
        Task ProcessRemindersAsync(CancellationToken cancellationToken = default);
    }
}
