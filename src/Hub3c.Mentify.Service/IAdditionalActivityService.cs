using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IAdditionalActivityService
    {
        void CreateOrUpdate(UserFieldOfStudy model);
    }
}
