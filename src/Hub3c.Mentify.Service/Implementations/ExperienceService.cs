using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Hub3c.Mentify.Service.Implementations
{
    public class ExperienceService : IExperienceService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<Experience> _experienceRepository;
        private IRepository<EduUser> _eduUserRepository;
        private IRepository<SystemUser> _systemUserRepository;
        private IRepository<DocumentRegister> _documentRegisterRepository;

        public ExperienceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _experienceRepository = unitOfWork.GetRepository<Experience>();
            _eduUserRepository = unitOfWork.GetRepository<EduUser>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _documentRegisterRepository = unitOfWork.GetRepository<DocumentRegister>();
        }

        public void Create(UserExperienceModel model)
        {
            if (model.IsCurrentlyWorkHere)
                UpdateOldCurrentWork(model.SystemUserId);
            if (model.Resume != null && !string.IsNullOrEmpty(model.Resume.UploadedFile))
            {
                UploadCv(model.SystemUserId, model.Resume);
            }
            _experienceRepository.Insert(new Experience()
            {
                CompanyName = model.CompanyName,
                Title = model.Title,
                EduExperienceInYears = model.StartYear,
                EduExperienceInMonths = model.StartMonth,
                EduExperienceInMonthsCompleted = (short?)model.EndMonth,
                EduExperienceInYearsCompleted = (short?)model.EndYear,
                SystemUserId = model.SystemUserId,
                IsCurrentlyWorkHere = model.IsCurrentlyWorkHere
            });
            _unitOfWork.SaveChanges();
        }

        private void UpdateOldCurrentWork(int systemUserId, int? id = null)
        {
            var currentWork = _experienceRepository.GetFirstOrDefault(predicate: a => a.IsCurrentlyWorkHere == true && a.SystemUserId == systemUserId);
            if (currentWork != null && currentWork.ExperienceId != id)
            {
                currentWork.IsCurrentlyWorkHere = false;
                _experienceRepository.Update(currentWork);
            }
        }

        private void UploadCv(int systemUserId, ResumeModel resume)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.EduUser));
            var eduUser = systemUser.EduUser.SingleOrDefault();

            var textAsBytes = Convert.FromBase64String(resume.UploadedFile);

            if (eduUser.ResumeDocumentId.HasValue)
            {
                var cv = _documentRegisterRepository.GetFirstOrDefault(predicate: a => a.DocumentId == eduUser.ResumeDocumentId);
                cv.DocumentMimeType = resume.MimeType;
                cv.DocumentName = resume.FileName;
                cv.DocumentSize = resume.Size;
                cv.DocumentContent = textAsBytes;
                cv.ModifiedBy = systemUserId;
                cv.ModifiedOn = DateTime.UtcNow;
                _documentRegisterRepository.Update(cv);
            }
            else
            {
                var document = new DocumentRegister()
                {
                    RowGuid = Guid.NewGuid(),
                    DocumentMimeType = resume.MimeType,
                    DocumentName = resume.FileName,
                    RegardingEntityName = Constant.STATIC_EDURESUMEDOC,
                    CreatedBy = systemUserId,
                    CreatedOn = DateTime.UtcNow,
                    DocumentContent = textAsBytes,
                    IsDirectory = false
                };
                _documentRegisterRepository.Insert(document);
                _unitOfWork.SaveChanges();
                eduUser.ResumeDocumentId = document.DocumentId;
                _eduUserRepository.Update(eduUser);
            }
        }

        public void Delete(int systemUserId, int id)
        {
            var experience = _experienceRepository.GetFirstOrDefault(predicate: a => a.ExperienceId == id);
            if (experience == null)
                throw new ApplicationException("The experience id is invalid");

            if (experience.SystemUserId != systemUserId)
                throw new ApplicationException("The experience is not yours.");

            _experienceRepository.Delete(experience);
            _unitOfWork.SaveChanges();
        }

        public void Edit(EditedUserExperienceModel model)
        {
            var experience = _experienceRepository.GetFirstOrDefault(predicate: a => a.ExperienceId == model.Id);
            if (experience == null)
                throw new ApplicationException("The experience id is invalid");

            if (model.Resume != null && !string.IsNullOrEmpty(model.Resume.UploadedFile))
                UploadCv(model.SystemUserId, model.Resume);

            if (model.IsCurrentlyWorkHere)
                UpdateOldCurrentWork(model.SystemUserId, experience.ExperienceId);

            experience.CompanyName = model.CompanyName;
            experience.Title = model.Title;
            experience.EduExperienceInYears = model.StartYear;
            experience.EduExperienceInYearsCompleted = (short?)model.EndYear;
            experience.EduExperienceInMonths = model.StartYear;
            experience.EduExperienceInMonthsCompleted = (short?)model.EndMonth;

            _experienceRepository.Update(experience);
            _unitOfWork.SaveChanges();
        }
    }
}
