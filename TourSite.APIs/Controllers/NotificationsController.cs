using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourSite.Core.DTOs.Notifications;
using TourSite.Core.DTOs.Common;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;

namespace TourSite.APIs.Controllers
{
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PageinationResponse<NotificationDto>>> GetMine(
            [FromQuery] bool unreadOnly = false, [FromQuery] PagedQueryDto query = null!)
        {
            query ??= new PagedQueryDto();
            var result = await _service.GetMineAsync(unreadOnly, query);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _service.GetUnreadCountAsync();
            return Ok(count);
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkRead(int id)
        {
            await _service.MarkReadAsync(id);
            return Ok(new { message = "Notification marked as read." });
        }

        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllRead()
        {
            await _service.MarkAllReadAsync();
            return Ok(new { message = "All notifications marked as read." });
        }

        [HttpGet("preferences")]
        public async Task<ActionResult<NotificationPreferenceDto>> GetPreferences()
        {
            var result = await _service.GetPreferencesAsync();
            return Ok(result);
        }

        [HttpPut("preferences")]
        public async Task<ActionResult<NotificationPreferenceDto>> UpdatePreferences([FromBody] NotificationPreferenceDto dto)
        {
            var result = await _service.UpdatePreferencesAsync(dto);
            return Ok(result);
        }
    }
}

