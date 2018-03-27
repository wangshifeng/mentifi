namespace Hub3c.Mentify.API.Models
{
    public class NewEducationViewModel
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public int ModeOfStudy { get; set; }
        public bool IsCurrentEducation { get; set; }
        public long? DateAttendedStart { get; set; }
        public long? DateAttendedEnd { get; set; }
    }

    public class EditedEducationViewModel : NewEducationViewModel
    {
        public int Id { get; set; }
    }
}
