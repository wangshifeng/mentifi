using System.Collections.Generic;

namespace Hub3c.Mentify.API.Models
{
    /// <summary>
    /// Authenticated User Model
    /// </summary>
    public class ProfileViewModel
    {
        #region SystemUser

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
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

        #endregion

        #region Edu User
        /// <summary>
        /// Student Number
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// I Identify as Aboriginal or Torres Strait Islander
        /// </summary>
        public bool IsAboriginalOrTorresStraisIslander { get; set; }
        /// <summary>
        /// I am an International Student
        /// </summary>
        public bool IsInterNationalStudent { get; set; }
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
        public bool IsHavingDisabilityActionPlan { get; set; }
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
        /// City
        /// </summary>
        public string City { get; set; }


        /// <summary>
        /// Attachment
        /// </summary>
        public int PlanDocumentId { get; set; }


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


        #endregion

        /// <summary>
        /// Educations
        /// </summary>
        public IEnumerable<EducationViewModel> Educations { get; set; }

        /// <summary>
        /// Additional Activities
        /// </summary>
        public IEnumerable<AdditionalActivity> AdditionalActivities { get; set; }

        /// <summary>
        /// Areas of specialisation
        /// </summary>
        public IEnumerable<EduSubjectExperience> EduSubjectExperiences { get; set; }

        /// <summary>
        /// I would like to mentor student studying
        /// </summary>
        public IEnumerable<EduSubjectPreference> EduSubjectPreferences { get; set; }

        /// <summary>
        /// Total mentor/ mentee of user
        /// </summary>
        public int MentorOrMenteeCount { get; set; }

    }

    /*select * from experience where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class Employment
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
    public class InformalExperience
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
    }

    /*select * from EduAdditionalActivity 
    join LookupTypeCode on EduAdditionalActivity.ActivityID = LookupTypeCode.AttributeValue and AttributeName = 'EduAdditionalActivity'
    where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class AdditionalActivity
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
    }

    /*
         select * from edusubjectpreference
        join LookupTypeCode on edusubjectpreference.FieldOfStudyID = LookupTypeCode.AttributeValue and AttributeName = 'EduSubject'
         where SystemUserID = 4137
         */
         /// <summary>
         /// Edu Subject Preference
         /// </summary>
    public class EduSubjectPreference
    {
        public string Name { get; set; }
        public string OtherName { get; set; }
    }


    /*
     select * from EduSubjectExperience
    join LookupTypeCode on EduSubjectExperience.FieldOfStudyID = LookupTypeCode.AttributeValue and AttributeName = 'EduSubject'
     where SystemUserID = 4137
    */
    /// <summary>
    /// Areas of specialisation
    /// </summary>
    public class EduSubjectExperience
    {

    }

    //Education Table
    /*select * from Education where SystemUserID = 4165*/
    /// <summary>
    /// 
    /// </summary>
    public class EducationViewModel
    {
        /// <summary>
        /// Field of Study
        /// </summary>
        public string FieldOfStudy { get; set; }
        /// <summary>
        /// 1 : Undergraduated
        /// 2 : Postgraduated
        /// </summary>
        public string Grade { get; set; }
        /// <summary>
        /// Degree
        /// </summary>
        public string Degree { get; set; }
        /// <summary>
        /// 1: Full time student
        /// 2: Part time student
        /// </summary>
        public string YearCompleted { get; set; }

        /// <summary>
        /// Is current education?
        /// </summary>
        public bool IsEduCurrentEducation { get; set; }
    }
}
