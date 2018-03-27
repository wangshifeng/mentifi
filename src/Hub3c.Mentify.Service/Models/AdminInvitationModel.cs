using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Service.Models
{
    public class AdminInvitationModel
    {
        [Required]
        public int AdminSystemUserId { get; set; }
        [Required]
        public string RegisterUrl { get; set; }

        public IEnumerable<AdminInvitationUserModel> AdminInvitationUserModels { get; set; }
    }

    public class AdminInvitationUserModel
    {
        [Required]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
