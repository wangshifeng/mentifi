using System.Collections.Generic;
using System.Threading.Tasks;
using Hub3c.Mentify.Service.Implementations;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IConnectionService
    {
        IEnumerable<ConnectionModel> GetMentee(int systemUserId, string url);
        IEnumerable<ConnectionModel> GetMentor(int systemUserId, string url);
        IEnumerable<ConnectionModel> GetConnected(int systemUserId, string url);
        IEnumerable<AdminModel> GetAdmin(int systemUserId, string url);
        IEnumerable<ConnectionModel> GetPending(int systemUserId, string url);
        IEnumerable<ConnectionModel> GetRequested(int systemUserId, string url);
        Task Request(int senderSystemUserId, int receiverSystemUserId, string message);
        Task Accept(int businessLinkId, int systemUserId);
        Task Reject(int businessLinkId, int systemUserId, string message);
        void Cancel(int senderSystemUserId, int receiverSystemUserId);
        void Remove(int senderSystemUserId, int receiverSystemUserId);
        ConnectionCountModel GetCounts(int systemUserId);
        AdminConnectionCountModel GetAdminCounts(int systemUserId);
        LookupModel<int> GetStatus(int fromSystemUserId, int toSystemUserId);
    }
}
