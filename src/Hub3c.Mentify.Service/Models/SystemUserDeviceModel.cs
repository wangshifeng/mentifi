using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Service.Models
{
    public class SystemUserDeviceModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Value for {0} must be greater than 0")]
        public int SystemUserId { get; set; }
        [Required]
        public string DeviceToken { get; set; }
    }
}
