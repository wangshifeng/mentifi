using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.API.Models
{
    public class NewExperienceViewModel
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public bool IsCurrentlyWorkHere { get; set; }
        public ResumeModel Resume { get; set; }

    }
    public class EditedExperienceViewModel : NewExperienceViewModel
    {
        public int Id { get; set; }
    }
}
