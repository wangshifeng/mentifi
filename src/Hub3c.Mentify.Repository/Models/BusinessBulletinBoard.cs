using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class BusinessBulletinBoard
    {
        [Key]
        public int BusinessPostId { get; set; }

        public int? RegardingPostId { get; set; }
        public int? SystemUserId { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public bool? IsNewPost { get; set; }
        public bool? IsBusinessOnly { get; set; }
        public DateTime? DatePosted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public int? Likes { get; set; }
        public int? RegardingBusinessId { get; set; }
        public bool? IsPrivatePost { get; set; }
        public string RecipientList { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string LikesList { get; set; }
        public int? Dislikes { get; set; }
        public string DislikesList { get; set; }
    }
}