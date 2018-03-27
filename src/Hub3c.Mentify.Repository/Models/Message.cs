using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class Message
    {
        [Key]
        public int HubMessageId { get; set; }
        public int ToBusinessId { get; set; }
        public int ToContactId { get; set; }
        public int FromBusinessId { get; set; }
        public int FromContactId { get; set; }
        public bool? ToAdviserEmail { get; set; }
        public bool? ToContactEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Priority { get; set; }
        public bool IsNewMessage { get; set; }
        public DateTime DateSent { get; set; }
        public string ToExternalEmail { get; set; }

        public SystemUser FromContact { get; set; }
        public SystemUser ToContact { get; set; }
    }
}
