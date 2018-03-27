using System.ComponentModel.DataAnnotations;
using Hub3c.Mentify.Repository.Models;

namespace Hub3c.Mentify.Service.Models
{
    public class NewTaskModel : BaseTaskModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int FromSystemUserId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ToSystemUserId { get; set; }
    }

    public class BaseTaskModel
    {
        [Required]
        public string Subject { get; set; }
        [Required]
        public MentifiTaskStatus Status { get; set; }
        [Required]
        public MentifiTaskPriority Priority { get; set; }

        public long? DueDate { get; set; }
        public int? AssignedToSystemUserId { get; set; }
    }

    public class EditedTaskModel : BaseTaskModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ModifiedBySystemUserId { get; set; }
    }

    public class TaskAssigneeModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ModifiedBySystemUserId { get; set; }
        public int? AssignedToSystemUserId { get; set; }
    }

    public class TaskStatusModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ModifiedBySystemUserId { get; set; }
        [Required]
        public MentifiTaskStatus Status { get; set; }
    }

}
