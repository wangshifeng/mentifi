namespace Hub3c.Mentify.Service.Models
{
    public class ProfileModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class ProfileModelIncludingEduType : ProfileModel
    {
        public LookupModel<int> EduBusinessType { get; set; }
    }
    public class ProfileModelIncludingEmail : ProfileModel
    {
        public string Email { get; set; }
    }
}
