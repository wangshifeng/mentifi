using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiChannelTask
    {
        [Key]
        public int MentifiChannelTaskId { get; set; }
        public int MentorId { get; set; }
        public int MenteeId { get; set; }

        public Business Mentor { get; set; }
        public Business Mentee { get; set; }
    }
}