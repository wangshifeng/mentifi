namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class ResetPassword
    {
        public string EmailAddress { get; set; }
        public string HostUrl { get; set; }
        public int SenderUserId { get; set; }
    }
}