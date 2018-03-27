using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hub3c.Mentify.Repository.Models
{
    public class EduInformalExperience 
    {
        [Key()]
        public int InformalExperienceId { get; set; }
        public int SystemUserId { get; set; }
        public int ExperienceId { get; set; }
        public string OtherExperience { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}