using System;
using System.Collections.Generic;

namespace Hub3c.Mentify.Repository.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int? BusinessId { get; set; }
        public int? BusinessTypeCode { get; set; }
        public int? PrimaryBusiness { get; set; }
        public string AccountName { get; set; }
        public string PrimaryContact { get; set; }
        public string WebsiteUrl { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Telephone3 { get; set; }
        public string Fax { get; set; }
        public string Abn { get; set; }
        public string Acn { get; set; }
        public string PostalLine1 { get; set; }
        public string PostalLine2 { get; set; }
        public string PostalLine3 { get; set; }
        public string PostalSuburb { get; set; }
        public string PostalPostCode { get; set; }
        public string PostalCity { get; set; }
        public string PostalState { get; set; }
        public string PostalCountry { get; set; }
        public string PhysicalLine1 { get; set; }
        public string PhysicalLine2 { get; set; }
        public string PhysicalLine3 { get; set; }
        public string PhysicalSuburb { get; set; }
        public string PhysicalPostCode { get; set; }
        public string PhysicalCity { get; set; }
        public string PhysicalState { get; set; }
        public string PhysicalCountry { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? ParentAccountId { get; set; }
        public int? AnnualRevenue { get; set; }
        public int? NumberOfEmployees { get; set; }
        public int? AssignedTo { get; set; }
        public int? RelationshipType { get; set; }
        public int? ClientStatus { get; set; }
        public DateTime? FollowupDate { get; set; }
        public DateTime? LastContacted { get; set; }
        public int? ServiceStatus { get; set; }
        public int? PaymentTerms { get; set; }
        public string GoogleLocation { get; set; }
        public string LocationLong { get; set; }
        public string LocationLat { get; set; }
        public string ReferredBy { get; set; }
        public bool? IsFavorite { get; set; }
        public int RegardingBusinessId { get; set; }
        public bool? IsSyncActive { get; set; }
        public bool? IsRestricted { get; set; }
        public bool IsInvited { get; set; }
        public int? InvitedByUserId { get; set; }

        public Business Business { get; set; }
    }
}