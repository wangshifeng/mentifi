using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub3c.Mentify.Repository.Models
{
    public class MentifiTask 
    {
        public int MentifiTaskId { get; set; }
        public string TaskSubject { get; set; }
        public int? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public MentifiTaskPriority Priority { get; set; }
        public MentifiTaskStatus Status { get; set; }
        public int Sequence { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int MentifiChannelTaskId { get; set; }

        [ForeignKey("AssignedTo")]
        public Business AssignedToBusiness { get; set; }
        [ForeignKey("CreatedBy")]
        public Business CreatedByBusiness { get; set; }
        public MentifiChannelTask MentifiChannelTask { get; set; }
    }
}