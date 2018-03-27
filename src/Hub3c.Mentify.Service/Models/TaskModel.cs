namespace Hub3c.Mentify.Service.Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public ProfileModel AssignedTo { get; set; }
        public long? DueDate { get; set; }
        public LookupModel<int> Priority { get; set; }
        public LookupModel<int> Status { get; set; }
        public int Sequence { get; set; }
        public ProfileModel Mentor { get; set; }
        public ProfileModel Mentee { get; set; }
    }

   
}
