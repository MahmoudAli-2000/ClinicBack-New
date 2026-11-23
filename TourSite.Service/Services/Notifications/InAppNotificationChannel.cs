using Microsoft.Extensions.Logging;
using TourSite.Core;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Service.Services.Notifications
{
    public class InAppNotificationChannel : INotificationChannelSender
    {
        public NotificationChannel Channel => NotificationChannel.InApp;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InAppNotificationChannel> _logger;

        public InAppNotificationChannel(IUnitOfWork unitOfWork, ILogger<InAppNotificationChannel> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SendAsync(Notification notification, CancellationToken ct = default)
        {
            switch (notification.Channel)
            {
                case NotificationChannel.Email:
                    _logger.LogInformation("Email notification queued: {Title} to {UserId}", notification.Title, notification.UserId);
                    break;
                case NotificationChannel.SMS:
                    _logger.LogInformation("SMS notification queued: {Title} to {UserId}", notification.Title, notification.UserId);
                    break;
                case NotificationChannel.Push:
                    _logger.LogInformation("Push notification queued: {Title} to {UserId}", notification.Title, notification.UserId);
                    break;
                case NotificationChannel.WhatsApp:
                    _logger.LogInformation("WhatsApp notification queued: {Title} to {UserId}", notification.Title, notification.UserId);
                    break;
                case NotificationChannel.InApp:
                    _logger.LogInformation("In-app notification for {UserId}: {Title}", notification.UserId, notification.Title);
                    break;
                default:
                    _logger.LogWarning("Unknown notification channel {Channel} for notification {Id}", notification.Channel, notification.Id);
                    break;
            }
        }
    }
}



