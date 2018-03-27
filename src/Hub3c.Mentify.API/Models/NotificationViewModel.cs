namespace Hub3c.Mentify.API.Models
{
    public class NotificationViewModel
    {
        public int SystemUserIdSender { get; set; }
        public int BusinessIdSender { get; set; }
        public string Content { get; set; }
        public string ProfileImage { get; set; }
    }
}
