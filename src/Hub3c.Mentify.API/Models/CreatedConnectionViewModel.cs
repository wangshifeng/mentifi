using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.API.Models
{
    /// <summary>
    /// Created Connection Model
    /// </summary>
    public class CreatedConnectionViewModel
    {
        /// <summary>
        /// System User ID of Sender
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int SenderSystemUserId { get; set; }

        /// <summary>
        /// System User ID of Receiver
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int ReceiverSystemUserId { get; set; }

        /// <summary>
        /// Message for sending email
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Request,Reject
        /// </summary>
        [Required]
        public ConnectionPostType ConnectionPostType { get; set; }

    }
}
