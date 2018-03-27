namespace Hub3c.Mentify.Service.Models
{
    public class NotificationModel
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public bool IsShowed { get; set; }
        public long CreatedOn { get; set; }
        public string NotificationTYpe { get; set; }
        public ProfileModelIncludingEduType Sender { get; set; }
    }
}
