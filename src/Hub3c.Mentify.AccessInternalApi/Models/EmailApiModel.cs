namespace Hub3c.Mentify.AccessInternalApi.Models
{
    public class EmailApiModel
    {
        public string[] Recipient { get; set; }
        public string[] Payload { get; set; }
        public string EmailType { get; set; }
        public string TemplateName { get; set; }
        public int SystemUserId { get; set; }
    }
}
