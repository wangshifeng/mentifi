using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Hub3c.Mentify.Service.Implementations
{
    public class SubjectPreferenceService : ISubjectPreferenceService
    {

        private IUnitOfWork _unitOfWork;
        private IRepository<EduSubjectPreference> _subjectPreferenceRepository;
        private IRepository<SystemUser> _systemUserRepository;
        private IRepository<EduUser> _eduUserRepository;

        public SubjectPreferenceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _subjectPreferenceRepository = unitOfWork.GetRepository<EduSubjectPreference>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _eduUserRepository = unitOfWork.GetRepository<EduUser>();
        }
        public void CreateOrUpdate(CreatedEduSubjectPreference model)
        {
            var subjectPreferences = _subjectPreferenceRepository
                 .GetPagedList(predicate: a => a.SystemUserId == model.SystemUserId, pageSize: int.MaxValue);

            if (subjectPreferences.TotalCount > 0)
                _subjectPreferenceRepository.Delete(subjectPreferences.Items);

            foreach (var informalExperience in model.FieldOfStudies)
            {
                _subjectPreferenceRepository.Insert(new EduSubjectPreference
                {
                    FieldOfStudyId = informalExperience.Id,
                    OtherFieldOfStudy = informalExperience.OtherValue,
                    SystemUserId = model.SystemUserId
                });
            }

            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.SystemUserId, include: a => a.Include(b => b.EduUser));
            if (systemUser == null)
                throw new ApplicationException("System User Id is invalid");

            var eduUser = systemUser.EduUser.SingleOrDefault();
            eduUser.PreferredMenteeGrade = model.PreferredMenteeGrade;
            _eduUserRepository.Update(eduUser);
            _unitOfWork.SaveChanges();
        }
    }
}
