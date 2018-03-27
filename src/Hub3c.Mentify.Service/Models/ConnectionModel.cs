using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class ConnectionModel
    {
        public int SystemUserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public ExperienceModel Occupation { get; set; }
        public LookupModel<int> MentifiType { get; set; }
        public string PreferredName { get; set; }
        public IEnumerable<LookupModel> SubjectPreferences { get; set; }
        public string Hobby { get; set; }
        public string MobilePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Email { get; set; }
        public int BusinessId { get; set; }
        public string Flag { get; set; }
        public string ProfilePhoto { get; set; }
        public LookupModel<string> Salutation { get; set; }
        public bool IsRejected { get; set; }
        public bool IsAbleToConnect { get; set; }
        public string SkillGoal { get; set; }
    }
}
