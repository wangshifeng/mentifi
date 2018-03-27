using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class AdditionalActivityService : IAdditionalActivityService
    {

        private IUnitOfWork _unitOfWork;
        private IRepository<EduAdditionalActivity> _additionalActivityRepository;

        public AdditionalActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _additionalActivityRepository = unitOfWork.GetRepository<EduAdditionalActivity>();
        }
        public void CreateOrUpdate(UserFieldOfStudy model)
        {
            var subjectPreferences = _additionalActivityRepository
                 .GetPagedList(predicate: a => a.SystemUserId == model.SystemUserId, pageSize: int.MaxValue);
            if (subjectPreferences.TotalCount > 0)
                _additionalActivityRepository.Delete(subjectPreferences.Items);

            foreach (var informalExperience in model.FieldOfStudies)
            {
                _additionalActivityRepository.Insert(new EduAdditionalActivity()
                {
                    
                    ActivityId = informalExperience.Id,
                    OtherActivityName = informalExperience.OtherValue,
                    SystemUserId = model.SystemUserId
                });
            }
            _unitOfWork.SaveChanges();
        }
    }
}
