using System;
using System.Collections.Generic;

namespace Hub3c.Mentify.Repository.Models
{
    public class SystemUser
    {
        public int SystemUserId { get; set; }
        public int BusinessId { get; set; }
        public int SystemUserType { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public int? PrimaryBusiness { get; set; }
        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string EmployeeId { get; set; }
        public bool? IsEnabled { get; set; }
        public bool IsInternalUser { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ExchangeServerAddress { get; set; }
        public bool? IsUsingFormAuthentication { get; set; }
        public string ExchangeUserName { get; set; }
        public string ExchangePassword { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public byte[] ProfilePhoto { get; set; }
        public string Dashboard1 { get; set; }
        public string Dashboard2 { get; set; }
        public string Dashboard3 { get; set; }
        public string Dashboard4 { get; set; }
        public string Expertise { get; set; }
        public string Bio { get; set; }
        public string Education { get; set; }
        public string HonoursAwards { get; set; }
        public string GroupsAssoc { get; set; }
        public string Location { get; set; }
        public string WebsiteUrl { get; set; }
        public string AuthRepNo { get; set; }
        public bool? IsUserInSales { get; set; }
        public bool? ShowHelpOnLoad { get; set; }
        public DateTime? Dob { get; set; }
        public int? MemberId { get; set; }
        public string SkypeName { get; set; }
        public bool? ShowCrmgettingStarted { get; set; }
        public string SocialPluginWidgets { get; set; }
        public string SocialPluginValues { get; set; }
        public int? DefaultHomePage { get; set; }
        public string FooterColour { get; set; }
        public string BackgroundColour { get; set; }
        public string GoogleCalendarLink { get; set; }
        public string AlternateEmailAddress { get; set; }
        public string DefaultTimezone { get; set; }
        public string FavouriteApplication { get; set; }
        public bool? ShowQuickTour { get; set; }
        public string Dashboard5 { get; set; }
        public string Dashboard6 { get; set; }
        public string Dashboard7 { get; set; }
        public string Dashboard8 { get; set; }
        public bool? OpenFavouriteInNewTab { get; set; }
        public short? UserLayoutPreference { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? Rating { get; set; }
        public bool ShowDocAccessPopup { get; set; }
        public string Rnversion { get; set; }
        public bool HasAdminPageAccess { get; set; }
        public int? AvailabilityStatus { get; set; }
        public DateTime? PhotoModifiedOn { get; set; }
        public int? PhotoModifiedBy { get; set; }
        public bool? IsShowPopup { get; set; }
        public bool IsActive { get; set; }
        public int RatingCount { get; set; }

        public Business Business { get; set; }
       
        public ICollection<EduAdditionalActivity> EduAdditionalActivity { get; set; }
        public ICollection<EduInformalExperience> EduInformalExperience { get; set; }
        public ICollection<EduSubjectExperience> EduSubjectExperience { get; set; }
        public ICollection<EduSubjectPreference> EduSubjectPreference { get; set; }
        public ICollection<EduUser> EduUser { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience> Experience { get; set; }
    }
}