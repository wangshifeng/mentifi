namespace Hub3c.Mentify.Service.Models
{
    public class ExperienceModel
    {
        public int ExperienceId { get; set; }
        public int SystemUserId { get; set; }
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public long? TimePeriodStart { get; set; }
        public long? TimePeriodEnd { get; set; }
        public string Description { get; set; }
        public int? EduExperienceInYears { get; set; }
        public int? EduExperienceInMonths { get; set; }
        public bool? IsCurrentlyWorkHere { get; set; }
        public int? EduExperienceInMonthsCompleted { get; set; }
        public int? EduExperienceInYearsCompleted { get; set; }
        public int Id { get; set; }
    }
}
