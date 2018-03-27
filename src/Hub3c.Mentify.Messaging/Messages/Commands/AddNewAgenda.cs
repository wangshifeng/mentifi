using System;

namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class AddNewAgenda
    {
        public int ProjectId { get; set; }
        public int SystemUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
