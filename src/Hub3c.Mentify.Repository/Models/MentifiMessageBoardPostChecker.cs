using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiMessageBoardPostChecker
    {
        [Key]
        public int PostId { get; set; }

        public int HubMessageId { get; set; }

        public int CreatedBy { get; set; }

        [ForeignKey("HubMessageId")]
        public MentifiMessage MentifiMessage { get; set; }

    }
}
