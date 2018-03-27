namespace Hub3c.Mentify.API.Models
{
    public class NetworkUserViewModel
    {
        public int SystemUserId { get; set; }
        public int BusinessId { get; set; }
        public string UrlPhoto { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// Current work history
        /// </summary>
        public string Occupation { get; set; }
    }
}
