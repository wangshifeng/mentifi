using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IMessageBoardService
    {
        void RequestConnection(RequestedConnectionMessageBoardModel model);
        void Create(NewMessageBoardModel model);

        PagedListModel<MessageBoardModel> Get(int fromSystemUserId, int toSystemUserId, string url, int limit,
            int pageIndex);
    }
}
