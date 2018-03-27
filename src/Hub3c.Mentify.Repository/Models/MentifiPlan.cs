using System.Collections.Generic;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiPlan 
    {
        public MentifiPlan()
        {
            MentifiPlanUser = new HashSet<MentifiPlanUser>();
            MentifiPlanUserHistory = new HashSet<MentifiPlanUserHistory>();
        }

        public int PlanId { get; set; }
        public string Description { get; set; }
        public int MaxUsers { get; set; }
        public decimal Cost { get; set; }
        public int PlanOrder { get; set; }

        public ICollection<MentifiPlanUser> MentifiPlanUser { get; set; }
        public ICollection<MentifiPlanUserHistory> MentifiPlanUserHistory { get; set; }
    }
}