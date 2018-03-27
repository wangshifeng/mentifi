using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IAdminInvitationService
    {
        void Send(AdminInvitationModel model);
    }
}
