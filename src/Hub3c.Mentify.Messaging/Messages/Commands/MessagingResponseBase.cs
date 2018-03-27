using System.Collections.Generic;
using System.Linq;

namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class MessagingResponseBase
    {
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public bool IsError => ErrorMessages.Any();
    }
}
