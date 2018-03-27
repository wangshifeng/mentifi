using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.API.Models
{
    public class NewGoalProgressViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ProgressValue { get; set; }
        [Required]
        public string Reason { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int MenteeSystemUserId { get; set; }
    }
}
