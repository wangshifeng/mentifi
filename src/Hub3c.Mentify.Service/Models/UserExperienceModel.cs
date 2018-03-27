namespace Hub3c.Mentify.Service.Models
{
    public class UserExperienceModel
    {
        public string CompanyName { get; set; }
        public string Title { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public int SystemUserId { get;  set; }
        public bool IsCurrentlyWorkHere { get;  set; }
        public ResumeModel Resume { get; set; }
    }

    public class ResumeModel
    {
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public int Size { get; set; }
        public string UploadedFile { get; set; }
    }

    public class EditedUserExperienceModel : UserExperienceModel
    {
        public int Id { get; set; }
    }

}
