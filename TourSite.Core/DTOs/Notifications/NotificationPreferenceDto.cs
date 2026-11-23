
namespace TourSite.Core.DTOs.Notifications
{
    public class NotificationPreferenceDto
    {
        public bool AllowInApp { get; set; }
        public bool AllowEmail { get; set; }
        public bool AllowSMS { get; set; }
        public bool AllowPush { get; set; }
        public bool AllowWhatsApp { get; set; }
        public bool AppointmentReminders { get; set; }
        public bool GeneralUpdates { get; set; }
    }
}
