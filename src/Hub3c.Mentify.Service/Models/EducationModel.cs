namespace Hub3c.Mentify.Service.Models
{
    public class UserEducationModel
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public int? ModeOfStudy { get; set; }
        public bool IsCurrentEducation { get; set; }
        public long? DateAttendedStart { get; set; }
        public long? DateAttendedEnd { get; set; }
        public int SystemUserId { get; set; }
    }

    public class EditedUserEducationModel : UserEducationModel
    {
        public int Id { get; set; }
    }
}
