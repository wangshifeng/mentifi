using System.Collections.Generic;

namespace Hub3c.Mentify.Messaging.Messages.Commands
{
    public class ResourceData
    {
        public ResourceData()
        {
            ResourceContents = new List<ResourceContent>();
        }
        public List<ResourceContent> ResourceContents { get; set; }
    }

    public class ResourceContent
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}