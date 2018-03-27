namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class AddGeneralNotification
    {
        public string NotificationType { get; set; }
        public int UserId { get; set; }
        public int? RegardingId { get; set; }
        public int CreatedBy { get; set; }
    }
}