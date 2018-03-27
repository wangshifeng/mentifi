namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class InviteTeamMember
    {
        public int SystemUserId { get; set; }
        public int BusinessId { get; set; }
        public string MailTo { get; set; }
        public string FullnameTo { get; set; }
        public string SystemPlan { get; set; }
        public string BusinessName { get; set; }
        public string FullnameFrom { get; set; }
        public string MailFrom { get; set; }
    }
}
