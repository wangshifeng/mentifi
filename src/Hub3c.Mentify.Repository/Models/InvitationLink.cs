using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class InvitationLink
    {
        [Key]
        public int InvitationLinkID { get; set; }

        public string UniqueUrl { get; set; }

        public int SystemUserID { get; set; }

        public int BusinessID { get; set; }

        public int InvitationType { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
