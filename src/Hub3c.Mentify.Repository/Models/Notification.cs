using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class Notification 
    {
        public int NotificationId { get; set; }
        public string NotificationType { get; set; }
        public int SystemUserId { get; set; }
        public string SystemUserType { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsShowed { get; set; }
        public int? RegardingId { get; set; }
        public int? CreatedBy { get; set; }
        public string Message { get; set; }
    }
}