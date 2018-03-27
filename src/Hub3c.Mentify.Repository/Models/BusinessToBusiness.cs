using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hub3c.Mentify.Repository.Models
{
    public class BusinessToBusiness 
    {
        [Key]
        public int BusinessLinkId { get; set; }
        [ForeignKey("Business1")]
        public int BusinessId1 { get; set; }
        public int? BusinessId2 { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPending { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public bool? IsClient { get; set; }
        public bool? IsSubsidiary { get; set; }
        public bool? IsNetwork { get; set; }
        public bool? IsSupplier { get; set; }
        public bool IsRejected { get; set; }

        public Business Business1 { get; set; }
    }
}