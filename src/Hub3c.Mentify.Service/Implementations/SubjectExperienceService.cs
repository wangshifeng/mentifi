using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class SubjectExperienceService : ISubjectExperienceService
    {

        private IUnitOfWork _unitOfWork;
        private IRepository<EduSubjectExperience> _subjectExperienceRepository;

        public SubjectExperienceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _subjectExperienceRepository = unitOfWork.GetRepository<EduSubjectExperience>();
        }
        public void CreateOrUpdate(UserFieldOfStudy model)
        {
            var subjectExperiences = _subjectExperienceRepository
                 .GetPagedList(predicate: a => a.SystemUserId == model.SystemUserId, pageSize: int.MaxValue);

            if (subjectExperiences.TotalCount > 0)
                _subjectExperienceRepository.Delete(subjectExperiences.Items);

            foreach (var informalExperience in model.FieldOfStudies)
            {
                _subjectExperienceRepository.Insert(new EduSubjectExperience()
                {
                    FieldOfStudyId = informalExperience.Id,
                    OtherFieldOfStudy = informalExperience.OtherValue,
                    SystemUserId = model.SystemUserId
                });
            }
            _unitOfWork.SaveChanges();
        }
    }
}
