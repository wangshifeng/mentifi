using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class EduSubjectExperience 
    {
        [Key()]
        public int SubjectExperienceId { get; set; }
        public int SystemUserId { get; set; }
        public int FieldOfStudyId { get; set; }
        public string OtherFieldOfStudy { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}