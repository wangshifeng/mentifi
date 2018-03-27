using System;
using System.Collections.Generic;

namespace Hub3c.Mentify.Service.Models
{

    /// <summary>
    /// Authenticated User Model
    /// </summary>
    public class UserProfileModel
    {
        #region SystemUser

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        public LookupModel<string> Salutation { get; set; }
        /// <summary>
        /// MID
        /// </summary>
        public int Mid { get; set; }
        /// <summary>
        /// System User Id
        /// </summary>
        public int SystemUserId { get; set; }
        /// <summary>
        /// Business ID
        /// </summary>
        public int BusinessId { get; set; }
        /// <summary>
        /// First Name
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Middle Name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Work Phone
        /// </summary>
        public string Workphone { get; set; }

        /// <summary>
        /// Mobile Phone
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// Photo Profile
        /// </summary>
        public string ProfilePhoto { get; set; }


        /// <summary>
        /// Gender
        /// </summary>
        public LookupModel<int> Gender { get; set; }

        #endregion

        #region Edu User
        /// <summary>
        /// Student Number
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// Attachment
        /// </summary>
        public Document PlanDocument { get; set; }

        /// <summary>
        /// I Identify as Aboriginal or Torres Strait Islander
        /// </summary>
        public bool? IsAboriginalOrTorresStraisIslander { get; set; }
        /// <summary>
        /// I am an International Student
        /// </summary>
        public bool? IsInterNationalStudent { get; set; }
        /// <summary>
        /// Country of Origin
        /// </summary>
        public string CountryOfOrigin { get; set; }
        /// <summary>
        /// Length of time in Australia
        /// </summary>
        public string TimeInAustralia { get; set; }
        /// <summary>
        /// I have a disability action plan
        /// </summary>
        public bool? IsHavingDisabilityActionPlan { get; set; }
        /// <summary>
        /// About Me, My Career Goals & Plans
        /// </summary>
        public string CareerGoal { get; set; }
        /// <summary>
        /// Professional Development Skills I Would Like to Develop with My Mentor
        /// </summary>
        public string SkillGoal { get; set; }
        /// <summary>
        /// My interests / hobbies are
        /// </summary>
        public string Hobby { get; set; }
        /// <summary>
        /// Brief Intro 
        /// </summary>
        public string BriefIntroduction { get; set; }

        /// <summary>
        /// 1: Full time student
        /// 2: Part time student
        /// </summary>
        public LookupModel<int> ModeOfStudy { get; set; }
        #endregion

        #region Business
        /// <summary>
        /// Address line 1
        /// </summary>
        public string AddressLine1 { get; set; }
        /// <summary>
        /// Address line 2
        /// </summary>
        public string AddressLine2 { get; set; }
        /// <summary>
        /// Address Line 3
        /// </summary>
        public string AddressLine3 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }


        public LookupModel<int> EduBusinessType { get; set; }


        /// <summary>
        /// Longitude
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// Latitude
        /// </summary>
        public string Latitude { get; set; }


        /// <summary>
        /// State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Post Code
        /// </summary>
        public string PostCode { get; set; }
        /// <summary>
        /// Country
        /// </summary>
        public string Country { get; set; }

        public bool? IsReferredToProgram { get; set; }
        public string ReferredBy { get; set; }
        public string GoogleLocation { get; set; }

        #endregion

        /// <summary>
        /// Educations
        /// </summary>
        public IEnumerable<EducationModel> Educations { get; set; }

        public int TotalMentorOrMentee { get; set; }
        /// <summary>
        /// Additional Activities
        /// </summary>
        public IEnumerable<AdditionalActivityModel> AdditionalActivities { get; set; }

        /// <summary>
        /// Areas of specialisation
        /// </summary>
        public IEnumerable<EduSubjectExperienceModel> EduSubjectExperiences { get; set; }

        /// <summary>
        /// I would like to mentor student studying
        /// </summary>
        public IEnumerable<EduSubjectPreferenceModel> EduSubjectPreferences { get; set; }
        public IEnumerable<ExperienceModel> Experiences { get; set; }
        public LookupModel<int> PreferredMenteeGrade { get; set; }
        public IEnumerable<ViewInformalExperienceModel> InformalExperiences { get; set; }
        public Document ResumeDocument { get; set; }
        public string PreferredName { get; set; }
    }

    public class Document
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
    }

    public class CurrentUser
    {
        /// <summary>
        /// MID
        /// </summary>
        public int Mid { get; set; }
        /// <summary>
        /// System User Id
        /// </summary>
        public int SystemUserId { get; set; }
        /// <summary>
        /// Business ID
        /// </summary>
        public int BusinessId { get; set; }

        public LookupModel<int> EduBusinessType { get; set; }
        public string FullName { get; set; }
        public string ProfilePhoto { get; set; }
        public string UniversityName { get; set; }
    }

    /*select * from experience where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class EmploymentModel
    {
        /// <summary>
        /// Company Name
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Is Currently Work Here?
        /// </summary>
        public string IsCurrentlyWorkHere { get; set; }

        /// <summary>
        /// Experience in year
        /// </summary>
        public int ExperienceInYear { get; set; }
        /// <summary>
        /// Experience in month
        /// </summary>
        public int ExperienceInMonth { get; set; }
    }

    /*
     select * from EduInformalExperience
     join LookupTypeCode on EduInformalExperience.ExperienceId = LookupTypeCode.AttributeValue and AttributeName = 'EduInformalExperience'
     where SystemUserID = 4137 
    */
    /// <summary>
    /// 
    /// </summary>
    public class ViewInformalExperienceModel
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
        public int Id { get; set; }
    }

    /*select * from EduAdditionalActivity 
    join LookupTypeCode on EduAdditionalActivity.ActivityID = LookupTypeCode.AttributeValue and AttributeName = 'EduAdditionalActivity'
    where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class AdditionalActivityModel
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
        public int Id { get; set; }
    }

    /*
         select * from edusubjectpreference
        join LookupTypeCode on edusubjectpreference.FieldOfStudyID = LookupTypeCode.AttributeValue and AttributeName = 'EduSubject'
         where SystemUserID = 4137
         */
    /// <summary>
    /// Edu Subject Preference
    /// </summary>
    public class LookupModel
    {
        public int SubjectExperienceId { get; set; }
        public string FieldOfStudy { get; set; }
        public string OtherFieldOfStudy { get; set; }
        public int FieldOfStudyId { get; set; }
    }

    /// <summary>
    /// Edu Subject Preference
    /// </summary>
    public class EduSubjectPreferenceModel
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
        public int Id { get; set; }
    }


    /*
     select * from EduSubjectExperienceModel
    join LookupTypeCode on EduSubjectExperienceModel.FieldOfStudyID = LookupTypeCode.AttributeValue and AttributeName = 'EduSubject'
     where SystemUserID = 4137
    */
    /// <summary>
    /// Areas of specialisation
    /// </summary>
    public class EduSubjectExperienceModel
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
        public int Id { get; set; }
    }

    //Education Table
    /*select * from Education where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class EducationModel
    {
        /// <summary>
        /// Field of Study
        /// </summary>
        public string FieldOfStudy { get; set; }
        /// <summary>
        /// 1 : Undergraduated
        /// 2 : Postgraduated
        /// </summary>
        public LookupModel<int> Grade { get; set; }
        /// <summary>
        /// Degree
        /// </summary>
        public string Degree { get; set; }


        /// <summary>
        /// Is current education?
        /// </summary>
        public bool? IsEduCurrentEducation { get; set; }

        public string Name { get; set; }
        public string YearCompleted { get; set; }
        public LookupModel<int> ModeOfStudy { get; set; }
        public long? DateAttendedStart { get; set; }
        public long? DateAttendedEnd { get; set; }
        public int? DateAttended { get; set; }
        public int Id { get; set; }
    }
}