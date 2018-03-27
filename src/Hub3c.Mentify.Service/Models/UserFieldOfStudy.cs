using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class UserFieldOfStudy
    {
        public int SystemUserId { get; set; }
        public IEnumerable<CreatedFieldOfStudy> FieldOfStudies { get; set; }
    }

    public class CreatedEduSubjectPreference : UserFieldOfStudy
    {
        public int PreferredMenteeGrade { get; set; }
    }

    public class CreatedFieldOfStudy
    {
        public int Id { get; set; }
        public string OtherValue { get; set; }
    }

  
}
