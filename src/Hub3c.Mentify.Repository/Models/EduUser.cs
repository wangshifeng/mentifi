using System;
using System.ComponentModel.DataAnnotations;

namespace Hub3c.Mentify.Repository.Models
{
    public class EduUser 
    {
        [Key]
        public Guid EduUserId { get; set; }
        public int SystemUserId { get; set; }
        public int Gender { get; set; }
        public bool? IsUniversityGraduate { get; set; }
        public bool? IsReferredToProgram { get; set; }
        public string ReferredBy { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string Hobby { get; set; }
        public bool? IsAvailableToMentorInternationalStudent { get; set; }
        public int PreferredMenteeGrade { get; set; }
        public int MatchedStatus { get; set; }
        public string BriefIntroduction { get; set; }
        public string PreferredName { get; set; }
        public string StudentNumber { get; set; }
        public bool? IsAboriginalOrTorresStraisIslander { get; set; }
        public bool? IsInternationalStudent { get; set; }
        public string CountryOfOrigin { get; set; }
        public string TimeInAustralia { get; set; }
        public bool? IsHavingDisabilityActionPlan { get; set; }
        public int ModeOfStudy { get; set; }
        public int? ResumeDocumentId { get; set; }
        public bool IsComplete { get; set; }
        public string CareerGoal { get; set; }
        public string SkillGoal { get; set; }
        public int? PlanDocumentId { get; set; }
        public bool IsBlocked { get; set; }
        public int? BlockedBy { get; set; }
        public DateTime? BlockedOn { get; set; }
        public bool IsHidden { get; set; }
        public int? HiddenBy { get; set; }
        public DateTime? HiddenOn { get; set; }

        public SystemUser SystemUser { get; set; }
    }
}