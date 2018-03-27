using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiApplicationUsage 
    {
        public int MentifiApplicationUsageId { get; set; }
        public int MentifiApplicationId { get; set; }
        public int BusinessId { get; set; }
        public int SystemUserId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}