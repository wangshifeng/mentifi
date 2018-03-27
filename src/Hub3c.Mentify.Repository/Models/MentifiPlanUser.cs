using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiPlanUser 
    {
        public int PlanUserId { get; set; }
        public int UniversityId { get; set; }
        public int MentifiPlanId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public MentifiPlan MentifiPlan { get; set; }
    }
}