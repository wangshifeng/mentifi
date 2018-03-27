using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IEducationService
    {
        void Create(UserEducationModel model);
        void Edit(EditedUserEducationModel model);
        void Delete(int systemUserId, int id);
    }
}
