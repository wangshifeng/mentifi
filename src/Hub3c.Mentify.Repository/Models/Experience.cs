using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class Experience 
    {
        public int ExperienceId { get; set; }
        public int SystemUserId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime? TimePeriodStart { get; set; }
        public DateTime? TimePeriodEnd { get; set; }
        public string Description { get; set; }
        public bool? IsCurrentlyWorkHere { get; set; }
        public int? EduExperienceInYears { get; set; }
        public int? EduExperienceInMonths { get; set; }
        public Int16? EduExperienceInMonthsCompleted { get; set; }
        public Int16? EduExperienceInYearsCompleted { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}