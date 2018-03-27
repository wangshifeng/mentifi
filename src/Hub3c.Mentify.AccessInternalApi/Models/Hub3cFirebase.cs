namespace Hub3c.Mentify.AccessInternalApi.Models
{
    // ReSharper disable once InconsistentNaming
    public class Hub3cFirebase
    {
        public string To { get; set; }
        public FIrebaseNotification Notification { get; set; }
        //public CustomModel Data { get; set; }
    }

    public class FIrebaseNotification
    {
        public string Body { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Sound { get; set; }
        public int Badge { get; set; }
        public NotificationType? Click_Action { get; set; }
        public bool mutable_content { get; set; }
    }

    public class CustomModel
    {
        public int NotificationId { get; set; }
    }
    public enum NotificationType
    {
        Pending = 1,
        Rejected = 2,
        Accepted = 3
    }
}
