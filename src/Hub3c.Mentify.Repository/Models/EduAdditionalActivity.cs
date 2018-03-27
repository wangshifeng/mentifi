using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class EduAdditionalActivity 
    {
        [Key]
        public int AdditionalActivityId { get; set; }
        public int SystemUserId { get; set; }
        public int ActivityId { get; set; }
        public string OtherActivityName { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}