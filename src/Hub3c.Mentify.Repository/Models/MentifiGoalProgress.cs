using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiGoalProgress 
    {
        [Key]
        public int MentifiGoalProgressId { get; set; }
        public int MentifiGoalId { get; set; }
        public int ProgressPercentage { get; set; }
        public string Reason { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Version { get; set; }

        public MentifiGoal MentifiGoal { get; set; }
    }
}