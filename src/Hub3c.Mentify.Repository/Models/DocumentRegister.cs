using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub3c.Mentify.Repository.Models
{
    public class DocumentRegister
    {
        [Key, Column("RowGuid", Order = 1)]
        public Guid RowGuid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("DocumentID", Order = 2)]
        public int DocumentId { get; set; }

        public int? RegardingEntityId { get; set; }
        public string RegardingEntityName { get; set; }
        public byte[] DocumentContent { get; set; }
        public string DocumentMimeType { get; set; }
        public string DocumentName { get; set; }
        public int? DocumentParentId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? DocumentSize { get; set; }
        public bool IsDirectory { get; set; }
    }
}
