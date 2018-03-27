using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiGoal 
    {
        [Key]
        public int MentifiGoalId { get; set; }
        public string GoalDescription { get; set; }
        public MentifiGoalProbability Probability { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Version { get; set; }

        public ICollection<MentifiGoalProgress> MentifiGoalProgress { get; set; }
    }
}