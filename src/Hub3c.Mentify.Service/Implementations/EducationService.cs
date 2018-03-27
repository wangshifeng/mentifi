using System;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class EducationService : IEducationService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<Education> _educationRepository;

        public EducationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _educationRepository = unitOfWork.GetRepository<Education>();
        }

        public void Delete(int systemUserId, int id)
        {
            var education = _educationRepository.GetFirstOrDefault(predicate: a => a.EducationId == id);
            if (education == null)
                throw new ApplicationException("The Education Id is invalid");
            if (education.SystemUserId != systemUserId)
                throw new ApplicationException("The education is not your own education");

            _educationRepository.Delete(education);
            _unitOfWork.SaveChanges();
        }

        public void Create(UserEducationModel model)
        {
            if (model.IsCurrentEducation)
                UpdateOldCurrentEducation(model.SystemUserId);

            _educationRepository.Insert(new Education()
            {
                School = model.School,
                Degree = model.Degree,
                SystemUserId = model.SystemUserId,
                DateAttendedEnd = model.DateAttendedEnd.FromUnixTime(),
                DateAttendedStart = model.DateAttendedStart.FromUnixTime(),
                IsEduCurrentEducation = model.IsCurrentEducation,
                Grade = model.ModeOfStudy.ToString()
            });
            _unitOfWork.SaveChanges();
        }

        public void Edit(EditedUserEducationModel model)
        {
            var education = _educationRepository.GetFirstOrDefault(predicate: a => a.EducationId == model.Id);
            if (education == null)
                throw new ApplicationException("The Education Id is invalid");

            if (model.IsCurrentEducation)
                UpdateOldCurrentEducation(model.SystemUserId, education.EducationId);

            education.School = model.School;
            education.Degree = model.Degree;
            education.SystemUserId = model.SystemUserId;
            education.DateAttendedEnd = model.DateAttendedEnd.FromUnixTime();
            education.DateAttendedStart = model.DateAttendedStart.FromUnixTime();
            education.IsEduCurrentEducation = model.IsCurrentEducation;
            education.Grade = model.ModeOfStudy.ToString();

            _educationRepository.Update(education);
            _unitOfWork.SaveChanges();
        }

        private void UpdateOldCurrentEducation(int systemUserId, int? educationId = null)
        {
            var currentEducation = _educationRepository.GetFirstOrDefault(predicate: a => a.IsEduCurrentEducation && a.SystemUserId == systemUserId);

            if (currentEducation != null && educationId.HasValue && currentEducation.EducationId != educationId)
            {
                currentEducation.IsEduCurrentEducation = false;
                _educationRepository.Update(currentEducation);
            }
        }
    }
}
