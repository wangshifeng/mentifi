using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Service.Models
{
    public class NewGoalModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public int ProbabilityId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int MenteeSystemUserId { get; set; }
    }

    public class EditedGoalModel: NewGoalModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int Id { get; set; }
    }
}
