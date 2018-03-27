using System;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Repository.Models.Notification> _notificationRepository;
        private readonly IRepository<Repository.Models.EduUniversity> _universityRepository;
        private readonly IRepository<Repository.Models.SystemUser> _systemUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHub3cFirebaseApi _hub3CFirebaseApi;
        private readonly ISystemUserDeviceService _systemUserDeviceService;
        private readonly IDashboardService _dashboardService;
        public NotificationService(IUnitOfWork unitOfWork, 
            IHub3cFirebaseApi hub3CFirebaseApi, 
            ISystemUserDeviceService systemUserDeviceService, 
            IDashboardService dashboardService)
        {
            _unitOfWork = unitOfWork;
            _hub3CFirebaseApi = hub3CFirebaseApi;
            _systemUserDeviceService = systemUserDeviceService;
            _dashboardService = dashboardService;
            _notificationRepository = unitOfWork.GetRepository<Repository.Models.Notification>();
            _universityRepository = unitOfWork.GetRepository<Repository.Models.EduUniversity>();
            _systemUserRepository = unitOfWork.GetRepository<Repository.Models.SystemUser>();
        }

        public int UnreadCount(int systemUserId, NotificationType notificationType)
        {
            var notifCode = GetNotifCode(notificationType);

            return _notificationRepository.Count(a =>
                   a.IsShowed == false && a.SystemUserId == systemUserId && a.NotificationType == notifCode);
        }

        public int UnreadCount(int systemUserId)
        {
            return _notificationRepository.Count(a =>
                a.IsShowed == false && a.SystemUserId == systemUserId);
        }

        public void Update(int systemUserId, NotificationType notificationType)
        {
            var notifCode = GetNotifCode(notificationType);
            var unreads = _notificationRepository.GetPagedList(predicate: a =>
                 a.IsShowed == false && a.SystemUserId == systemUserId && a.NotificationType == notifCode);
            foreach (var unread in unreads.Items)
            {
                unread.IsShowed = true;
                _notificationRepository.Update(unread);
            }
            _unitOfWork.SaveChanges();
        }

        public void UpdateAll(int systemUserId)
        {
            var unreads = _notificationRepository.GetPagedList(predicate: a =>
                a.IsShowed == false && a.SystemUserId == systemUserId);
            foreach (var unread in unreads.Items)
            {
                unread.IsShowed = true;
                _notificationRepository.Update(unread);
            }
            _unitOfWork.SaveChanges();
        }

        public void SendNotificationFromWeb(MobileAppNotification notification)
        {
            if (notification.SystemUserId == 0)
                return;

            var devices = _systemUserDeviceService.GetBySystemUserId(notification.SystemUserId);
            var systemUser =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == notification.SystemUserId,
                    include: a => a.Include(b => b.Business));
            var badges = UnreadCount(notification.SystemUserId);
            var university = _universityRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));
            var createdBy =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == notification.CreatedBy);

            var message = _dashboardService.ResourceMessage(new Repository.Models.Notification()
            {
                SystemUserId = notification.SystemUserId,
                NotificationType = notification.Notification,
                Message = notification.Message,
                CreatedBy = notification.CreatedBy,
                RegardingId = notification.RegardingID
            }, createdBy, university, systemUser, false).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(message))
            {
                foreach (var device in devices)
                {
                    var response = _hub3CFirebaseApi.Send(new Hub3cFirebase()
                    {
                        Notification = new FIrebaseNotification()
                        {
                            Title = "Mentifi",
                            Body = message,
                            Icon = "default",
                            Sound = "default",
                            Badge = badges,
                            mutable_content = true,
                            Click_Action = GetNotifType(notification.Notification)
                        },
                        To = device.DeviceToken
                    }).GetAwaiter().GetResult();
                }
            }
        }

        private NotificationType? GetNotifType(string notificationType)
        {
            switch (notificationType)
            {
                case Constant.NOTIFTYPE_BUSINESSLINK:
                    return NotificationType.Pending;
                case Constant.NOTIFTYPE_LINKACTIVATE:
                    return NotificationType.Accepted;
                case Constant.NOTIFTYPE_EDUREJECT:
                    return NotificationType.Rejected;
                default:
                    return null;
            }
        }


        public void Update(int notificationId)
        {
            var unreads = _notificationRepository.GetFirstOrDefault(predicate: a =>
                a.NotificationId == notificationId);
            if (unreads == null) throw new ApplicationException("Notification ID is invalid");

            unreads.IsShowed = true;
            _notificationRepository.Update(unreads);
            _unitOfWork.SaveChanges();
        }

        private string GetNotifCode(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Accepted:
                    return Constant.NOTIFTYPE_EDUACCEPTCONNECT;

                case NotificationType.Rejected:
                    return Constant.NOTIFTYPE_EDUREJECT;

                case NotificationType.Pending:
                    return Constant.NOTIFTYPE_EDUCONNECT;

                default:
                    return string.Empty;
            }
        }
    }
}
