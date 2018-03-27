using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class SystemUserDevice
    {
        [Key]
        public int DeviceId { get; set; }
        public int SystemUserId { get; set; }
        public string DeviceToken { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}