using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IExperienceService
    {
        void Create(UserExperienceModel model);
        void Edit(EditedUserExperienceModel model);
        void Delete(int systemUserId, int id);
    }
}
