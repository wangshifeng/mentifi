using Hub3c.Mentify.Service.Models;
using System.Collections.Generic;

namespace Hub3c.Mentify.API.Models
{
    public class EduSubjectPreferenceViewModel
    {
        public IEnumerable<CreatedFieldOfStudy> FieldOfStudies { get; set; }
        public int PreferredMenteeGrade { get; set; }
    }
}
