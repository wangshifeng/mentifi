using System;
using System.Collections.Generic;
using System.Text;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class InformalExperienceService : IInformalExperienceService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<EduInformalExperience> _informalExperienceRepository;

        public InformalExperienceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _informalExperienceRepository = _unitOfWork.GetRepository<EduInformalExperience>();
        }

        public void CreateOrUpdate(UserInformalExperienceModel model)
        {
            var informalExperiences = _informalExperienceRepository
                .GetPagedList(predicate: a => a.SystemUserId == model.SystemUserId, pageSize: int.MaxValue);

            if (informalExperiences.TotalCount > 0)
                _informalExperienceRepository.Delete(informalExperiences.Items);

            foreach (var informalExperience in model.InformalExperiences)
            {
                _informalExperienceRepository.Insert(new EduInformalExperience()
                {
                    ExperienceId = informalExperience.Id,
                    OtherExperience = informalExperience.OtherValue,
                    SystemUserId = model.SystemUserId
                });
            }
            _unitOfWork.SaveChanges();
        }
    }
}
