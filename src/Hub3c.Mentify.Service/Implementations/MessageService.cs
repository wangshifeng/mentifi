using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hub3c.Mentify.Service.Implementations
{
    public class MessageService : IMessageService
    {

        private readonly IRepository<MentifiMessageBoardPostChecker> _messageCheckerRepository;
        private readonly IRepository<Message> _messageRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IBusInstance _busInstance;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailApi _emailApi;
        private readonly IConfiguration _configuration;
        public MessageService(IUnitOfWork unitOfWork, IBusInstance busInstance, IEmailApi emailApi, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _busInstance = busInstance;
            _emailApi = emailApi;
            _configuration = configuration;
            _messageCheckerRepository = unitOfWork.GetRepository<MentifiMessageBoardPostChecker>();
            _notificationRepository = unitOfWork.GetRepository<Notification>();
            _businessToBusinessRepository = unitOfWork.GetRepository<BusinessToBusiness>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _messageRepository = unitOfWork.GetRepository<Message>();
        }

        public PagedListModel<MessageModel> GetInbox(int systemUserId, int pageIndex, int limit, string baseUrl)
        {
            var x = _messageRepository.GetPagedList(predicate: a =>
                   a.ToBusinessId == systemUserId ,
                orderBy: a => a.OrderByDescending(b => b.DateSent), pageIndex: pageIndex,
                pageSize: limit);

            var fromIds = x.Items.Select(a => a.FromBusinessId).Distinct().ToArray();
            var toIds = x.Items.Select(a => a.ToBusinessId).Distinct().ToArray();
            var froms = _systemUserRepository.GetPagedList(predicate: a => fromIds.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            var tos = _systemUserRepository.GetPagedList(predicate: a => toIds.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            var y = x.Items.Select(a => Mapping(a, baseUrl, froms, tos));
            return y.Map(x);    
        }

        public void SetAsRead(int id)
        {
            var message = _messageRepository.GetFirstOrDefault(predicate: a => a.HubMessageId == id);
            if (message == null)
                throw new ApplicationException("The message id is invalid");
            message.IsNewMessage = false;
            _messageRepository.Update(message);
            _unitOfWork.SaveChanges();
        }

        public void Create(NewMessageModel model)
        {
            var message = new Message()
            {
                Body = model.Body,
                FromBusinessId = model.FromSystemUserId,
                ToBusinessId = model.ToSystemUserId,
                DateSent = DateTime.UtcNow,
                Subject = model.Subject,
                Priority = "Normal"
            };
            _messageRepository.Insert(message);

            var to = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.ToSystemUserId);
            var from = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.FromSystemUserId, include: a => a.Include(b => b.Business));
            if (model.EmailMessage)
            {
                _emailApi.NewMessage(new EmailParam
                {
                    SystemUserId = model.FromSystemUserId,
                    Recipient = new[]
                    {
                        to.EmailAddress
                    },
                    Payload = new[]
                    {
                        to.FullName,
                        from.FullName,
                        from.Business.BusinessName,
                        model.Body,
                        _configuration["MentifiWebUrl"]
                    }
                }).GetAwaiter().GetResult();
            }
            _unitOfWork.SaveChanges();
        }

        public PagedListModel<MessageModel> GetOutbox(int systemUserId, int pageIndex, int limit, string baseUrl)
        {

            var messages = _messageRepository.GetPagedList(predicate: a =>
                    (a.FromBusinessId == systemUserId),
                orderBy: a => a.OrderByDescending(b => b.DateSent),
                pageIndex: pageIndex, pageSize: limit);
            var fromIds = messages.Items.Select(a => a.FromBusinessId).Distinct().ToArray();
            var toIds = messages.Items.Select(a => a.ToBusinessId).Distinct().ToArray();
            var froms = _systemUserRepository
                .GetPagedList(predicate: a => fromIds.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            var tos = _systemUserRepository
                .GetPagedList(predicate: a => toIds.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            var y = messages.Items.Select(a => Mapping(a, baseUrl, froms, tos));
            return y.Map(messages);
        }

        public void Delete(int id)
        {
            var message = _messageRepository.GetFirstOrDefault(predicate: a => a.HubMessageId == id);
            if (message == null)
                throw new ApplicationException("The id is invalid.");
            _messageRepository.Delete(message);
            _unitOfWork.SaveChanges();
        }

        public MessageModel GetById(int id, string baseUrl)
        {
            var message = _messageRepository.GetFirstOrDefault(predicate: a => a.HubMessageId == id);
            var from = _systemUserRepository.GetPagedList(predicate: a =>
                a.SystemUserId == (message.FromContactId == 0 ? message.FromBusinessId : message.FromContactId)).Items;
            var to = _systemUserRepository.GetPagedList(predicate: a =>
                a.SystemUserId == (message.ToContactId == 0 ? message.ToBusinessId : message.ToContactId)).Items;
            return Mapping(message, baseUrl, from, to);
        }

        public MessageModel Mapping(Message message, string url, IEnumerable<SystemUser> froms, IEnumerable<SystemUser> tos)
        {
            return new MessageModel()
            {
                Id = message.HubMessageId,
                Message = message.Body,
                Subject = message.Subject,
                Receiver = tos.FirstOrDefault(a => a.SystemUserId == (message.ToContactId == 0 ? message.ToBusinessId : message.ToContactId))?.ToProfileModelIncludingEmail(url),
                Sender = froms.FirstOrDefault(a => a.SystemUserId == (message.FromContactId == 0 ? message.FromBusinessId : message.FromContactId))?.ToProfileModelIncludingEmail(url),
                CreatedDate = message.DateSent.ToUnixTime(),
                IsNew = message.IsNewMessage
            };
        }
        public MessageModel Mapping(Message message, string url)
        {
            return new MessageModel()
            {
                Id = message.HubMessageId,
                Message = message.Body,
                Subject = message.Subject,
                Receiver = message.ToContact.ToProfileModelIncludingEmail(url),
                Sender = message.FromContact.ToProfileModelIncludingEmail(url),
                CreatedDate = message.DateSent.ToUnixTime(),
                IsNew = message.IsNewMessage
            };
        }
    }
}
