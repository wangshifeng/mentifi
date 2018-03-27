namespace Hub3c.Mentify.Service.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public int? SystemUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public long? CreatedOn { get; set; }
    }
}
