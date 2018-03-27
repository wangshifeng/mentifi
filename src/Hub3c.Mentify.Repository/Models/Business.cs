using System;
using System.Collections.Generic;

namespace Hub3c.Mentify.Repository.Models
{
    public class Business
    {
        public int BusinessId { get; set; }
        public int DealerId { get; set; }
        public string BusinessName { get; set; }
        public string Acn { get; set; }
        public string Abn { get; set; }
        public int? BusinessTypeCode { get; set; }
        public int? PrimaryBusiness { get; set; }
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public string WebsiteUrl { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Fax { get; set; }
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
        public int? HubPlanId { get; set; }
        public string BusinessSummary { get; set; }
        public string Bio { get; set; }
        public string History { get; set; }
        public string GroupsAssoc { get; set; }
        public string HonoursAwards { get; set; }
        public string Expertise { get; set; }
        public byte[] ProfileLogo { get; set; }
        public bool? IsProfileActive { get; set; }
        public bool? IsProfilePublic { get; set; }
        public int? RegardingCharity { get; set; }
        public string Afsl { get; set; }
        public string LetterheadFileName { get; set; }
        public string DealerName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? SubscribeDate { get; set; }
        public int? Likes { get; set; }
        public bool? IsStrategicPartner { get; set; }
        public string BannerPath { get; set; }
        public bool? IsBusinessAccount { get; set; }
        public string NetworkId { get; set; }
        public string RelatedNetworkId { get; set; }
        public int? PlanUserId { get; set; }
        public DateTime? DocumentStorageExpirationDate { get; set; }
        public bool? IsNetworkRewardParticipant { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Bsb { get; set; }
        public bool? IsAcceptNrp { get; set; }
        public bool? IsGstregistered { get; set; }
        public DateTime? LastImportDate { get; set; }
        public string EventPaymentAccount { get; set; }
        public string StorePaymentAccount { get; set; }
        public int? InvitedBy { get; set; }
        public string LandingPageKey { get; set; }
        public string MailChimpAccessToken { get; set; }
        public bool? IsExpress { get; set; }
        public string BasAgentNo { get; set; }
        public bool? IsCompleteEngagementWizard { get; set; }
        public int? ExpressContactId { get; set; }
        public int? ExpressJoinEngagementType { get; set; }
        public bool? IsCompleteSignLetterWizard { get; set; }
        public int? XeroIntegrationType { get; set; }
        public string XeroAccountPayable { get; set; }
        public string XeroAccountReceivable { get; set; }
        public bool? HasApplyTurnKeyData { get; set; }
        public decimal? DefaultTaxPercentage { get; set; }
        public int? BusinessTypeId { get; set; }
        public int? BillingType { get; set; }
        public DateTime? BillingDate { get; set; }
        public string ToolbarBackgroundColour { get; set; }
        public string FooterColour { get; set; }
        public string GoogleLocation { get; set; }
        public string LocationLong { get; set; }
        public string LocationLat { get; set; }
        public string Alias { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripePlanId { get; set; }
        public string StripeSubscriptionId { get; set; }
        public DateTime? LogoModifiedOn { get; set; }
        public int? LogoModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public int PrimaryContactId { get; set; }
        public int EduBusinessType { get; set; }
        public int? UniversityId { get; set; }

        public BusinessType BusinessType { get; set; }
    
        public EduUniversity EduUniversity { get; set; }
        public ICollection<Account> Account { get; set; }
        public ICollection<BusinessToBusiness> BusinessToBusiness { get; set; }
        public ICollection<SystemUser> SystemUser { get; set; }
    }
}