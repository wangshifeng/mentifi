using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi.Models;

namespace Hub3c.Mentify.AccessInternalApi
{
    public interface IEmailApi
    {
        Task<object> RequestConnection(EmailParam model);
        Task<object> RejectConnection(EmailParam model);
        Task<object> AcceptConnection(EmailParam model);
        Task<object> AddGoalProgress(EmailParam model);
        Task<object> NewMessage(EmailParam model);
        Task<object> InviteAdmin(EmailParam model);
    }
}
