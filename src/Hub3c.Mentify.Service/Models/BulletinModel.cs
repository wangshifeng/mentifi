using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class BulletinModel
    {
        public int Id { get; set; }
        public int? SystemUserId { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public long? CreatedOn { get; set; }
        public long? ModifiedOn { get; set; }
        public string FullName { get; set; }
        public int CommentCount { get; set; }
        public bool EnableLike { get; set; }
        public bool EnableDisLike { get; internal set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public IEnumerable<AttachmentModel> Attachments { get; set; }
        public string ProfilePhotoUrl { get; set; }
    }
}
