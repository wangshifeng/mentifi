using System;

namespace Hub3c.Messaging.Message
{
    public class MobileAppNotification
    {
        public int NotificationID { get; set; }
        public string Notification { get; set; }
        public string NotificationText { get; set; }
        public string InvitedBy { get; set; }
        public string Description { get; set; }
        public int? RegardingID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Message { get; set; }
        public int NewNotification { get; set; }

        public int SystemUserId { get; set; }
    }
}