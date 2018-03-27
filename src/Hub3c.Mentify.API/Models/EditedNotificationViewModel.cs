using System.ComponentModel.DataAnnotations;
using Hub3c.Mentify.AccessInternalApi.Models;

namespace Hub3c.Mentify.API.Models
{
    public class EditedNotificationViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int SystemUserId { get; set; }
        [Required]
        public NotificationType NotificationType { get; set; }
    }
}
