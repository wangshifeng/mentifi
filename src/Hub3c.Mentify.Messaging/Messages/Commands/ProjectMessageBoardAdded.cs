namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class ProjectMessageBoardAdded
    {
        public int ProjectId { get; set; }
        public int? UserId { get; set; }
    }
}