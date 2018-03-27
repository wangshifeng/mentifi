using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class UserInformalExperienceModel
    {
        public int SystemUserId { get; set; }
        public IEnumerable<InformalExperienceModel> InformalExperiences { get; set; }
    }

    public class InformalExperienceModel
    {
        public int Id { get; set; }
        public string OtherValue { get; set; }
    }
}
