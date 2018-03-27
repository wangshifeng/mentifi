using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IMessageService
    {
        PagedListModel<MessageModel> GetInbox(int systemUserId, int pageIndex, int limit, string baseUrl);
        void SetAsRead(int id);
        void Create(NewMessageModel model);
        PagedListModel<MessageModel> GetOutbox(int systemUserId, int pageIndex, int limit, string baseUrl);
        void Delete(int id);
        MessageModel GetById(int id, string baseUrl);
    }
}
