using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using EduUser = Hub3c.Mentify.Repository.Models.EduUser;

namespace Hub3c.Mentify.Service.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<EduUniversity> _eduUniversityRepository;
        private readonly IRepository<DocumentRegister> _documentRegisterRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly IRepository<EduUser> _eduUserRepository;
        private readonly ILookupService _lookupService;
        private readonly IConnectionService _connectionService;
        private readonly IUnitOfWork _unitOfWork;
        public UserProfileService(IUnitOfWork unitOfWork, ILookupService lookupService, IConnectionService connectionService)
        {
            _lookupService = lookupService;
            _connectionService = connectionService;
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _eduUniversityRepository = unitOfWork.GetRepository<EduUniversity>();
            _businessRepository = unitOfWork.GetRepository<Business>();
            _documentRegisterRepository = unitOfWork.GetRepository<DocumentRegister>();
            _eduUserRepository = unitOfWork.GetRepository<EduUser>();
            _unitOfWork = unitOfWork;
        }

        public CurrentUser Get(int mid, string baseUrl)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.MemberId == mid && a.Business.EduBusinessType != 0,
                include: a => a.Include(b => b.Business));
            var university = _eduUniversityRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId), include: a => a.Include(b => b.Business));

            if (systemUser == null) throw new ApplicationException("The User is not mentifi user");

            return new CurrentUser
            {
                SystemUserId = systemUser.SystemUserId,
                BusinessId = systemUser.BusinessId,
                Mid = mid,
                EduBusinessType = MentifiTypeLookup(systemUser.Business.EduBusinessType),
                FullName = systemUser.FullName,
                ProfilePhoto = systemUser.ToPhotoUrl(baseUrl),
                UniversityName = university.Business.BusinessName
            };
        }


        public UserProfileModel GetBySystemUserId(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business)
                .ThenInclude(h => h.BusinessToBusiness)
                .Include(c => c.EduUser)
                .Include(d => d.Educations)
                .Include(e => e.Experience)
                .Include(f => f.EduSubjectExperience)
                .Include(g => g.EduSubjectPreference)
                .Include(g => g.EduInformalExperience)
                .Include(h => h.EduAdditionalActivity));

            if (systemUser == null) throw new ApplicationException("System User ID is not found");
            var eduBusiness = systemUser.Business;
            if (eduBusiness == null) throw new ApplicationException("Edu user is not found");
            var eduUser = systemUser.EduUser.FirstOrDefault();
            if (eduUser == null) throw new ApplicationException("Edu user is not found");

            return new UserProfileModel
            {
                SystemUserId = systemUser.SystemUserId,
                LastName = systemUser.LastName,
                FirstName = systemUser.FirstName,
                BusinessId = systemUser.BusinessId,
                MiddleName = systemUser.MiddleName,
                PreferredName = eduUser.PreferredName,
                Salutation = Salutation(systemUser.Title),
                IsReferredToProgram = eduUser.IsReferredToProgram,
                ReferredBy = eduUser.ReferredBy,
                ProfilePhoto = systemUser.ToPhotoUrl(url),
                Mid = systemUser.MemberId ?? 0,
                Email = systemUser.EmailAddress,
                Workphone = systemUser.WorkPhone,
                MobilePhone = systemUser.MobilePhone,
                AddressLine1 = eduBusiness.PhysicalLine1,
                AddressLine2 = eduBusiness.PhysicalLine2,
                AddressLine3 = eduBusiness.PhysicalLine3,
                City = eduBusiness.PhysicalCity,
                Latitude = eduBusiness.LocationLat,
                Longitude = eduBusiness.LocationLong,
                State = eduBusiness.PhysicalState,
                PostCode = eduBusiness.PhysicalPostCode,
                Country = eduBusiness.PhysicalCountry,
                EduBusinessType = MentifiTypeLookup(eduBusiness.EduBusinessType),
                StudentNumber = eduUser.StudentNumber,
                PlanDocument = MappingDocument(eduUser.PlanDocumentId, url),
                ResumeDocument = MappingDocument(eduUser.ResumeDocumentId, url),
                IsAboriginalOrTorresStraisIslander = eduUser.IsAboriginalOrTorresStraisIslander,
                IsInterNationalStudent = eduUser.IsInternationalStudent,
                CountryOfOrigin = eduUser.CountryOfOrigin,
                TimeInAustralia = eduUser.TimeInAustralia,
                IsHavingDisabilityActionPlan = eduUser.IsHavingDisabilityActionPlan,
                CareerGoal = eduUser.CareerGoal,
                SkillGoal = eduUser.SkillGoal,
                Hobby = eduUser.Hobby,
                BriefIntroduction = eduUser.BriefIntroduction,
                ModeOfStudy = ModeOfStudy(eduUser.ModeOfStudy),
                Educations = Educations(systemUser, eduUser),
                Experiences = Experiences(systemUser),
                EduSubjectExperiences = EduSubjectExperience(systemUser),
                EduSubjectPreferences = EduSubjectPreferences(systemUser),
                AdditionalActivities = EduAdditionalActivity(systemUser),
                InformalExperiences = InformalExperiences(systemUser),
                TotalMentorOrMentee = TotalConnected(systemUser),
                Gender = Gender(eduUser.Gender),
                PreferredMenteeGrade = PreferredMenteeGrade(systemUser.EduUser),
                GoogleLocation = systemUser.Business.GoogleLocation
            };
        }


        private Document MappingDocument(int? documentId, string url)
        {
            if (!documentId.HasValue || documentId == 0)
                return null;

            return _documentRegisterRepository.GetFirstOrDefault(selector: a => new Document()
            {
                Id = a.DocumentId,
                Name = a.DocumentName,
                Url = a.DocumentId.ToDocumentUrl(url),
                Extension = a.DocumentMimeType
            }, predicate: a => a.DocumentId == documentId);

        }
        private LookupModel<int> PreferredMenteeGrade(ICollection<EduUser> systemUserEduUser)
        {
            var eduUser = systemUserEduUser.FirstOrDefault();
            if (eduUser?.PreferredMenteeGrade == 0)
            {
                return null;
            }
            var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduGrade);
            return new LookupModel<int>(
                lookups.FirstOrDefault(b => b.Id == eduUser?.PreferredMenteeGrade)?.Name ?? string.Empty,
                eduUser?.PreferredMenteeGrade ?? 0);
        }

        private LookupModel<string> Salutation(string title)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            var id = int.Parse(title);
            var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_Salutation);
            return new LookupModel<string>(
                lookups.FirstOrDefault(b => b.Id == id)?.Name ?? string.Empty,
                title);
        }

        public LookupModel<int> Gender(int value)
        {
            switch (value)
            {
                case 1:
                    return new LookupModel<int>("Male", 1);

                case 2:
                    return new LookupModel<int>("Female", 2);
                default: return null;
            }
        }

        public byte[] GetPhoto(int id)
        {
            return _systemUserRepository.GetFirstOrDefault(a => a.ProfilePhoto, a => a.SystemUserId == id);
        }

        public LookupModel<int> GetMentifiType(int businessId)
        {
            var business = _businessRepository.GetFirstOrDefault(predicate: a => a.BusinessId == businessId);
            if (business != null)
                return MentifiTypeLookup(business.EduBusinessType);

            throw new ApplicationException("User is not mentifi user");

        }

        public UserSettingModel GetUserSetting(int systemUserId)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(b => b.BusinessToBusiness));

            var universitySetting = _eduUniversityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));

            if (universitySetting == null) throw new ApplicationException("University ID is invalid.");

            var model = new UserSettingModel
            {
                IsMentorAllowedToSearchMentee = universitySetting.IsMentorAllowedToSearchMentee,
                MaxMenteeRequestedSent = universitySetting.MaxMenteeRequest,
                MaxNumberMenteeForMentor = universitySetting.MaxNumberMenteeForMentor,
                MaxNumberMentorForMentee = universitySetting.MaxNumberMentorForMentee,
                MenteeAlias = universitySetting.MenteeAlias,
                MentorAlias = universitySetting.MentorAlias,
                ProgramName = universitySetting.ProgramName,
                UniverisityNameAlias = universitySetting.UniversityNameAlias,
            };
            var count = _connectionService.GetCounts(systemUserId);
            model.ConnectedCount = count.Connected;
            model.RequestedSentCount = count.Pending;
            return model;
        }

        public void Edit(EditedUserProfileModel model)
        {
            var user = _systemUserRepository
                .GetFirstOrDefault(predicate: a => a.SystemUserId == model.SystemUserId,
                    include: a => a.Include(b => b.Business).Include(c => c.EduUser));

            if (user == null)
                throw new ApplicationException("System user id is invalid");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.FullName = $"{model.FirstName} {model.LastName}";

            var eduUser = user.EduUser.SingleOrDefault();
            eduUser.PreferredName = model.PreferredName;
            eduUser.Gender = model.Gender;
            _eduUserRepository.Update(eduUser);

            user.MobilePhone = model.Mobilephone;
            user.WorkPhone = model.Workphone;

            if (!string.IsNullOrEmpty(model.ProfilePhoto))
            {
                var textAsBytes = Convert.FromBase64String(model.ProfilePhoto);
                user.ProfilePhoto = textAsBytes;
            }

            _systemUserRepository.Update(user);
            _unitOfWork.SaveChanges();
        }

        private int TotalConnected(SystemUser systemUser)
        {
            return systemUser.Business.BusinessToBusiness.Count(a => a.IsActive == true && a.BusinessId2 != systemUser.Business.UniversityId);
        }

        private LookupModel<int> MentifiTypeLookup(int eduBusinessType)
        {
            switch (eduBusinessType)
            {
                case 1:
                    return new LookupModel<int>("Mentor", 1);
                case 2:
                    return new LookupModel<int>("Mentee", 2);
                case 3:
                    return new LookupModel<int>("Admin", 3);
                default:
                    throw new ApplicationException("User is not mentifi user");
            }
        }
        private LookupModel<int> ModeOfStudy(int modeOfStudy)
        {
            switch (modeOfStudy)
            {
                case 1:
                    return new LookupModel<int>("Full time", 1);
                case 2:
                    return new LookupModel<int>("Part time", 2);
                default:
                    return null;
            }
        }
        private IEnumerable<EducationModel> Educations(SystemUser systemUser, EduUser eduUser)
        {
            return systemUser.Educations.Select(a => new EducationModel
            {
                Id = a.EducationId,
                Degree = a.Degree,
                FieldOfStudy = a.FieldOfStudy,
                Grade = EduGrade(a.Grade),
                IsEduCurrentEducation = a.IsEduCurrentEducation,
                Name = a.School,
                YearCompleted = a.YearCompleted,
                ModeOfStudy = a.IsEduCurrentEducation ? ModeOfStudy(eduUser.ModeOfStudy) : null,
                DateAttendedStart = a.DateAttendedStart.ToUnixTime(),
                DateAttendedEnd = a.DateAttendedEnd.ToUnixTime(),
                DateAttended = (a.DateAttendedStart.HasValue && a.DateAttendedEnd.HasValue) ? (int)(a.DateAttendedEnd.Value.Subtract(a.DateAttendedStart.Value).TotalDays / 365.25) : 0
            }).ToList();
        }

        private LookupModel<int> EduGrade(string grade)
        {
            return !string.IsNullOrEmpty(grade) ? _lookupService.GetByAttribute(Constant.LookupTypeCode_EduGrade, int.Parse(grade)) : null;
        }


        private IEnumerable<EduSubjectPreferenceModel> EduSubjectPreferences(SystemUser systemUser)
        {
            var subjectPreferences = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject);
            return systemUser.EduSubjectPreference.Select(a => new EduSubjectPreferenceModel
            {
                Id = a.SubjectPreferenceId,
                Name = subjectPreferences.FirstOrDefault(b => b.Id == a.FieldOfStudyId)?.Name,
                OtherName = a.OtherFieldOfStudy
            }).ToList();
        }

        private IEnumerable<EduSubjectExperienceModel> EduSubjectExperience(SystemUser systemUser)
        {
            var subjectPreferences = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject);
            return systemUser.EduSubjectExperience.Select(a => new EduSubjectExperienceModel
            {
                Id = a.SubjectExperienceId,
                Name = subjectPreferences.FirstOrDefault(b => b.Id == a.FieldOfStudyId)?.Name,
                OtherName = a.OtherFieldOfStudy
            }).ToList();
        }

        private IEnumerable<AdditionalActivityModel> EduAdditionalActivity(SystemUser systemUser)
        {
            var additionalActivities = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduAdditionalActivity);

            return systemUser.EduAdditionalActivity.Select(a => new AdditionalActivityModel
            {
                Id = a.AdditionalActivityId,
                Name = additionalActivities.FirstOrDefault(b => b.Id == a.ActivityId)?.Name,
                OtherName = a.OtherActivityName
            }).ToList();

        }

        private IEnumerable<ViewInformalExperienceModel> InformalExperiences(SystemUser systemUser)
        {
            var informalExperiences = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduInformalExperience);

            return systemUser.EduInformalExperience.Select(a => new ViewInformalExperienceModel
            {
                Id = a.InformalExperienceId,
                Name = informalExperiences.FirstOrDefault(b => b.Id == a.ExperienceId)?.Name,
                OtherName = a.OtherExperience
            }).ToList();

        }

        private IEnumerable<ExperienceModel> Experiences(SystemUser systemUser)
        {
            return systemUser.Experience.Select(a => new ExperienceModel
            {
                Id = a.ExperienceId,
                SystemUserId = a.SystemUserId,
                TimePeriodStart = a.TimePeriodStart?.ToUnixTime(),
                ExperienceId = a.ExperienceId,
                EduExperienceInMonths = a.EduExperienceInMonths,
                TimePeriodEnd = a.TimePeriodEnd?.ToUnixTime(),
                Location = a.Location,
                EduExperienceInYears = a.EduExperienceInYears,
                EduExperienceInYearsCompleted = a.EduExperienceInYearsCompleted,
                EduExperienceInMonthsCompleted = a.EduExperienceInMonthsCompleted,
                Description = a.Description,
                CompanyName = a.CompanyName,
                Title = a.Title,
                IsCurrentlyWorkHere = a.IsCurrentlyWorkHere
            }).ToList();
        }

        public void EditBiography(EditedBiography model)
        {
            var user = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.SystemUserId, include: a => a.Include(b => b.EduUser));
            if (user == null)
                throw new ApplicationException("System user id is invalid.");

            var eduuser = user.EduUser.SingleOrDefault();
            eduuser.BriefIntroduction = model.BriefIntroduction;
            eduuser.Hobby = model.Hobby;
            _eduUserRepository.Update(eduuser);
            _unitOfWork.SaveChanges();
        }

        public void EditPersonalGoal(EditedPersonalGoal model)
        {
            var user = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.SystemUserId, include: a => a.Include(b => b.EduUser));
            if (user == null)
                throw new ApplicationException("System user id is invalid.");

            var eduuser = user.EduUser.SingleOrDefault();
            eduuser.CareerGoal = model.CareerGoal;
            eduuser.SkillGoal = model.SkillGoal;
            _eduUserRepository.Update(eduuser);
            _unitOfWork.SaveChanges();
        }
    }

}