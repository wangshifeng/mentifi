namespace Hub3c.Mentify.AccessInternalApi.Models
{
    public class EmailParam
    {
        public string[] Recipient { get; set; }
        public string[] Payload { get; set; }
        public int SystemUserId { get; set; }
    }
}
