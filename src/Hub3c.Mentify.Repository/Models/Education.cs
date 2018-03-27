using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class Education 
    {
        public int EducationId { get; set; }
        public int SystemUserId { get; set; }
        public string School { get; set; }
        public DateTime? DateAttendedStart { get; set; }
        public DateTime? DateAttendedEnd { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public string Grade { get; set; }
        public string ActivitiesAndSocieties { get; set; }
        public string Description { get; set; }
        public string YearCompleted { get; set; }
        public bool IsEduCurrentEducation { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}