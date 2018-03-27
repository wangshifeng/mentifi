using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Service.Models
{
    public class EditedUserProfileModel
    {
        public int SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get;  set; }
        public string PreferredName { get; set; }
        public int Gender { get; set; }
        public string Workphone { get; set; }
        public string Mobilephone { get; set; }
        public string ProfilePhoto { get; set; }
    }

    public class EditedBiography
    {
        public int SystemUserId { get; set; }
        public string BriefIntroduction { get; set; }
        [MaxLength(200, ErrorMessage = "Hobby cannot be longer than 200 characters")]
        public string Hobby { get; set; }
    }

    public class EditedPersonalGoal
    {
        public int SystemUserId { get; set; }
        [MaxLength(200, ErrorMessage = "Career goal cannot be longer than 200 characters")]
        public string CareerGoal { get; set; }
        public string SkillGoal { get; set; }
    }
}
