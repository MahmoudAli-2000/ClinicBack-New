using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public NotificationChannel Channel { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; }
        public int? AppointmentId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? HiddenAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
    }
}
