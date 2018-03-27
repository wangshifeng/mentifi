using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface ISubjectExperienceService
    {
        void CreateOrUpdate(UserFieldOfStudy model);
    }
}
