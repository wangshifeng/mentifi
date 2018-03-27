using System;

namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class InvitationCalendarNewActivity
    {
        public string ProjectName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string RecipientFullName { get; set; }
        public string RecipientEmailAddress { get; set; }
        public string SenderFullName { get; set; }
        public string SenderEmailAddress { get; set; }
        public int SystemUserId { get; set; }
        public DateTime ProposedStartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReminderDate { get; set; }
    }
}