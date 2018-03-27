using Hub3c.Mentify.Repository.Models;

namespace Hub3c.Mentify.API.Models
{
    public class EditedTaskViewModel
    {
        public string Subject { get; set; }
        public MentifiTaskStatus Status { get; set; }
        public MentifiTaskPriority Priority { get; set; }
        public long? DueDate { get; set; }
        public int? AssignedToSystemUserId { get; set; }
        public int ModifiedBySystemUserId { get; set; }
    }

    public class AssigningTaskViewModel
    {
        public int? AssignedToSystemUserId { get; set; }
        public int ModifiedBySystemUserId { get; set; }
    }

    public class TaskStatusViewModel
    {
        public MentifiTaskStatus Status { get; set; }
        public int ModifiedBySystemUserId { get; set; }
    }
}
