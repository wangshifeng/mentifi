using System;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class MessageBoardService : IMessageBoardService
    {
        private readonly IRepository<MentifiMessage> _messageRepository;
        private readonly IRepository<MentifiMessageBoardPostChecker> _mentifiMessageBoardPostRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IBusInstance _busInstance;
        private readonly IUnitOfWork _unitOfWork;
        public MessageBoardService(IUnitOfWork unitOfWork, IBusInstance busInstance)
        {
            _unitOfWork = unitOfWork;
            _busInstance = busInstance;
            _notificationRepository = unitOfWork.GetRepository<Notification>();
            _businessToBusinessRepository = unitOfWork.GetRepository<BusinessToBusiness>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _messageRepository = unitOfWork.GetRepository<MentifiMessage>();
            _mentifiMessageBoardPostRepository = unitOfWork.GetRepository<MentifiMessageBoardPostChecker>();
        }

        public void RequestConnection(RequestedConnectionMessageBoardModel model)
        {

            var message = new MentifiMessage()
            {
                Subject = "Mentor/Mentee Connection Request",
                Body = model.Body,
                DateSent = DateTime.UtcNow,
                FromBusinessId = model.FromBusinessId,
                FromSystemUserId = model.FromSystemUserId,
                ToSystemUserId = model.ToSystemUserId,
                ToBusinessId = model.ToBusinessId,
            };

            var messagePostChecker = new MentifiMessageBoardPostChecker()
            {
                CreatedBy = model.FromSystemUserId,
                HubMessageId = message.MentifiMessageId,
                MentifiMessage = message
            };
            _mentifiMessageBoardPostRepository.Insert(messagePostChecker);
            CreateNotif(model.FromSystemUserId, model.FromSystemUserId, Constant.NOTIFTYPE_MENTIFIMESSAGEBOARDREQUESTSENT,
                model.ToSystemUserId.ToString());
        }

        public void Create(NewMessageBoardModel model)
        {
            var from =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.FromSystemUserId,
                    include: a => a.Include(b => b.Business));

            if (from == null)
                throw new ApplicationException("From Id is invalid.");

            var to =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.ToSystemUserId,
                    include: a => a.Include(b => b.Business));
            if (to == null)
                throw new ApplicationException("To Id is invalid.");

            var businessToBusiness = _businessToBusinessRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId1 == from.BusinessId && a.BusinessId2 == to.BusinessId); if (businessToBusiness == null)
                throw new ApplicationException("The Connection is not found.");

            if (businessToBusiness.IsPending == false)
                throw new ApplicationException("Your connection is not pending anymore");
            var requester = businessToBusiness.CreatedBy == from.SystemUserId ? from : to;
            var accepter = requester.SystemUserId == from.SystemUserId ? to : from;

            var message = new MentifiMessage
            {
                Subject = "Message Board Post",
                Body = model.Body,
                DateSent = DateTime.UtcNow,
                FromBusinessId = requester.BusinessId,
                FromSystemUserId = requester.SystemUserId,
                ToSystemUserId = accepter.SystemUserId,
                ToBusinessId = accepter.BusinessId,
            };
            var messagePostChecker = new MentifiMessageBoardPostChecker()
            {
                CreatedBy = model.FromSystemUserId,
                HubMessageId = message.MentifiMessageId,
                MentifiMessage = message
            };
            _mentifiMessageBoardPostRepository.Insert(messagePostChecker);
            CreateNotif(model.ToSystemUserId, model.FromSystemUserId, Constant.NOTIFTYPE_MENTIFIMESSAGEBOARD,
                message.MentifiMessageId.ToString());

            _unitOfWork.SaveChanges();
        }

        public PagedListModel<MessageBoardModel> Get(int fromSystemUserId, int toSystemUserId, string url, int limit, int pageIndex)
        {
            var from =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId,
                    include: a => a.Include(b => b.Business));

            if (from == null)
                throw new ApplicationException("From Id is invalid.");

            var to =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId,
                    include: a => a.Include(b => b.Business));
            if (to == null)
                throw new ApplicationException("To Id is invalid.");

            var businessToBusiness = _businessToBusinessRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId1 == from.BusinessId && a.BusinessId2 == to.BusinessId);

            if (businessToBusiness == null)
                throw new ApplicationException("The Connection is not found.");

            var fromSystemUser = businessToBusiness.CreatedBy == from.SystemUserId ? from : to;
            var toSystemUser = fromSystemUser.SystemUserId == from.SystemUserId ? to : from;

            return _messageRepository.GetPagedList(selector: a => Mapping(a, from, to, url), predicate:
                 a => a.FromSystemUserId == fromSystemUser.SystemUserId && a.ToSystemUserId == toSystemUser.SystemUserId && a.MentifiMessageBoardPostChecker != null,
                 include: a => a.Include(b => b.FromSystemUser).Include(c => c.ToSystemUser).Include(d => d.MentifiMessageBoardPostChecker),
                 orderBy: a => a.OrderByDescending(b => b.DateSent), pageIndex: pageIndex, pageSize: limit).Map();
        }

        private MessageBoardModel Mapping(MentifiMessage message, SystemUser from, SystemUser to, string url)
        {
            return new MessageBoardModel()
            {
                Id = message.MentifiMessageId,
                Message = message.Body,
                CreatedBy = message.MentifiMessageBoardPostChecker.CreatedBy == from.SystemUserId ? from.ToProfileModel(url):to.ToProfileModel(url),
                CreatedDate = message.DateSent.ToUnixTime()
            };
        }

        private void CreateNotif(int systemUserId, int createdBy, string notifType, string message)
        {
            var model = new Notification
            {
                SystemUserId = systemUserId,
                CreatedOn = DateTime.Now,
                CreatedBy = createdBy,
                SystemUserType = Constant.USERTYPE_CONTACT,
                NotificationType = notifType,
                IsShowed = false,
                Message = message
            };
            _notificationRepository.Insert(model);

            _busInstance.Publish(new NotificationAdded
            {
                NotificationType = notifType,
                SystemUserID = systemUserId,
                SystemUserType = Constant.USERTYPE_CONTACT,
                CreatedOn = DateTime.UtcNow,
                IsShowed = false,
                CreatedBy = createdBy,
                Message = message
            });
        }
    }
}
