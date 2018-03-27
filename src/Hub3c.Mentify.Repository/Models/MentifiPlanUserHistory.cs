using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiPlanUserHistory 
    {
        public int PlanUserHistoryId { get; set; }
        public int UniversityId { get; set; }
        public int MentifiPlanId { get; set; }
        public int PurchasedBy { get; set; }
        public DateTime Subscribed { get; set; }
        public int Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public MentifiPlan MentifiPlan { get; set; }
    }
}