using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class EduUniversity 
    {
        [Key()]
        public int BusinessId { get; set; }
        public string UniversityNameAlias { get; set; }
        public string ProgramName { get; set; }
        public string MentorAlias { get; set; }
        public string MenteeAlias { get; set; }
        public int MaxNumberMenteeForMentor { get; set; }
        public int MaxNumberMentorForMentee { get; set; }
        public bool? IsMentorAllowedToSearchMentee { get; set; }
        public int MaxMenteeRequest { get; set; }

        public Business Business { get; set; }
    }
}