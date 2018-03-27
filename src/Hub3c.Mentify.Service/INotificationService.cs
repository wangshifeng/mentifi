using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Messaging.Messages.Commands;
using Hub3c.Messaging.Message;

namespace Hub3c.Mentify.Service
{
    public interface INotificationService
    {
        int UnreadCount(int systemUserId, NotificationType notificationType);
        int UnreadCount(int systemUserId);
        void Update(int systemUserId, NotificationType notificationType);
        void Update(int notificationId);
        void UpdateAll(int systemUserId);

        void SendNotificationFromWeb(MobileAppNotification notification);
    }
}
