namespace Hub3c.Mentify.Service.Models
{
    public class NewGoalProgressModel
    {
        public int ProgressValue { get; set; }
        public string Reason { get; set; }
        public int MenteeSystemUserId { get; set; }
        public int GoalId { get; set; }
    }
}
