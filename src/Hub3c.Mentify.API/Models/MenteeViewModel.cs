namespace Hub3c.Mentify.API.Models
{
    public class MenteeViewModel
    {
        public int SystemUserId { get; set; }
        public int BusinessId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Skill { get; set; }
        public bool IsConnected { get; set; }
    }
}
