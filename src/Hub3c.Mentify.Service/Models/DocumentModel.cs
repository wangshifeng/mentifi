namespace Hub3c.Mentify.Service.Models
{
    public class DocumentModel : AttachmentModel
    {
        public byte[] Content { get; set; }
        public string Mime { get; set; }
    }
}
