using System.ComponentModel.DataAnnotations;
using TourSite.Core.DTOs.Common;
namespace TourSite.Core.DTOs.Notifications;
public class NotificationQueryDto : PagedQueryDto {
 [RegularExpression("^(all|appointments|reminders|general)$")] public string Category { get; set; } = "all";
 public bool Hidden { get; set; } = false;
}
