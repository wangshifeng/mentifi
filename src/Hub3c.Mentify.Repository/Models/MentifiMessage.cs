using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiMessage
    {
        [Key]
        public int MentifiMessageId { get; set; }
        public int ToBusinessId { get; set; }
        public int ToSystemUserId { get; set; }
        public int FromBusinessId { get; set; }
        public int FromSystemUserId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }

        [ForeignKey("FromSystemUserId")]
        public SystemUser FromSystemUser { get; set; }
        [ForeignKey("ToSystemUserId")]
        public SystemUser ToSystemUser { get; set; }

        public MentifiMessageBoardPostChecker MentifiMessageBoardPostChecker { get; set; }
    }
}
