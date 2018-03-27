namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class ReminderTeamMemberNewActivity
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string EmailTo { get; set; }
        public string FullName { get; set; }
        public string BusinessName { get; set; }
        public string FullNameFrom { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectName { get; set; }
        public string ProjectRole { get; set; }
        public string HostUrl { get; set; }
        public string LabelSetCode { get; set; }
    }
}
