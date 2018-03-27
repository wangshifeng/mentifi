using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hub3c.Mentify.Repository.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        public int? MentifiTaskId { get; set; }

        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int StatusTypeCode { get; set; }
        public DateTime? ActualStart { get; set; }
        public DateTime? ActualEnd { get; set; }
        public DateTime? ProposedStart { get; set; }
        public DateTime? ProposedEnd { get; set; }
        public int? RegardingLead { get; set; }
        public int? RegardingActivity { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public SystemUser CreatedBySystemUser { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedByType { get; set; }
        public decimal? PercentageCOmplete { get; set; }
        public string Currency { get; set; }
        public int? RegardingOpportunity { get; set; }
        public int? RegardingTask { get; set; }
        public int? RegardingAccount { get; set; }

        public string ProjectCode  { get; set; }
        public int? ServiceType  { get; set; }
        public int? RecurringProjectId { get; set; }

        public decimal? FixedPriceAmount { get; set; }
        public int BillingType { get; set; }
        public bool? IsSynchronizeInvoice { get; set; }
        public bool? IsArchive { get; set; }
        public bool? UseActivityCustomColor { get; set; }
        public int? ApprovalStatus { get; set; }
        public double? PreferableAvgRatePerHour { get; set; }
        public int? SkillCategoryId { get; set; }
        public bool? IsShowedDeclineMessageInEmployer { get; set; }
        public bool? IsDeleted { get; set; }
        public int? RegardingAccountSource { get; set; }

        public ICollection<ProjectTeam> ProjectTeams { get; set; }
    }
}
