using System;

namespace Hub3c.Messaging.Message
{
    public class NotificationAdded
    {
        public int SystemUserID { get; set; }
        public string SystemUserType { get; set; }
        public string NotificationType { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsShowed { get; set; }
        public int RegardingID { get; set; }
        public int CreatedBy { get; set; }
        public string LabelCode { get; set; }
        public string Message { get; set; }
    }
}
