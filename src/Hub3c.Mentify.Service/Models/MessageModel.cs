using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Service.Models
{
    public class MessageModel
    {
        public int Id { get; set; }
        public ProfileModel Receiver { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public long CreatedDate { get; set; }
        public bool IsNew { get; set; }
        public ProfileModel Sender { get; set; }
    }

    public class RequestedConnectionMessageBoardModel
    {
        public int ToBusinessId { get; set; }
        public int ToSystemUserId { get; set; }
        public int FromBusinessId { get; set; }
        public int FromSystemUserId { get; set; }
        public string Body { get; set; }
    }

    public class NewMessageBoardModel
    {
        [Required]
        public int ToSystemUserId { get; set; }
        [Required]
        public int FromSystemUserId { get; set; }
        [Required]
        public string Body { get; set; }
    }

    public class NewMessageModel
    {
        [Required]
        public int ToSystemUserId { get; set; }
        [Required]
        public int FromSystemUserId { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public string Subject { get; set; }

        public bool EmailMessage { get; set; }
    }

    public class MessageBoardModel
    {
        public int Id { get; set; }
        public ProfileModel CreatedBy { get; set; }
        public string Message { get; set; }
        public long CreatedDate { get; set; }
    }
}
