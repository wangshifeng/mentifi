using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class MeetingAttendee 
    {
        public int MeetingAttendeeId { get; set; }
        public int MeetingId { get; set; }
        public int? SystemUserId { get; set; }
        public bool? IsAttending { get; set; }
        public bool? IsJoined { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? RegardingTeamMemberId { get; set; }
    }
}