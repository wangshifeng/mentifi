using System;

namespace Hub3c.Mentify.Repository.Models
{
    public class EmailQueue 
    {
        public int EmailQueueId { get; set; }
        public string RecipientEmail { get; set; }
        public string TemplatePath { get; set; }
        public string TemplateData { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public string TemplateType { get; set; }
        public bool? IsFailed { get; set; }
        public string ExceptionMessage { get; set; }
        public string RecipientEmailCc { get; set; }
    }
}