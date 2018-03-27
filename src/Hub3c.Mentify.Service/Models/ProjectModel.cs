using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public ProfileModel CreatedBy { get; set; }
    }
}
