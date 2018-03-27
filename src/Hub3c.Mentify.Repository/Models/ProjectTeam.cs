using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class ProjectTeam
    {
        [Key]
        public int ProjectTeamId { get; set; }
        public int ProjectId { get; set; }
        public int? UserId { get; set; }
        public string UserType { get; set; }
        public string ProjectRole { get; set; }
        public string NonMemberName { get; set; }
        public string NonMemberBusiness { get; set; }
        public string NonMemberEmail { get; set; }
        public bool? IsProjectOwner { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByType { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedType { get; set; }
        public decimal? HourylyRate { get; set; }
        public decimal? HourylyRateSUb { get; set; }
        public bool? CanApproveInvoice { get; set; }
        public decimal Rating { get; set; }
        public DateTime? LastRateDate { get; set; }
        public bool IsRatingSUbmitted { get; set; }

        public Project Project { get; set; }

        public SystemUser User { get; set; }
    }
}
