using System;

namespace Hub3c.Mentify.Messaging.Messages.Events
{
    public class BusinessApproved
    {
        public int BusinessApprovedId { get; set; }
        public int BusinesssApprovedById { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}