using TourSite.Core.Entities;
using System.Threading;
using System.Threading.Tasks;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Servicies.Contract
{
    public interface INotificationChannelSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
    }
}
