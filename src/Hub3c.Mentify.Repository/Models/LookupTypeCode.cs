using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class LookupTypeCode 
    {
        [Key]
        public int TypeCodeId { get; set; }
        public int BusinessId { get; set; }
        public int EntityTypeCode { get; set; }
        public string AttributeName { get; set; }
        public int AttributeValue { get; set; }
        public string Value { get; set; }
        public int? DisplayOrder { get; set; }
        public byte[] VersionNumber { get; set; }
    }
}