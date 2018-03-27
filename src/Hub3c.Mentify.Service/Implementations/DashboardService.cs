using System;
using Hub3c.Mentify.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hub3c.Mentify.Core.Utility;
using Hub3c.Mentify.MongoRepo;
using Hub3c.Mentify.MongoRepo.Model;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Notification = Hub3c.Mentify.Repository.Models.Notification;
using SystemUser = Hub3c.Mentify.Repository.Models.SystemUser;

namespace Hub3c.Mentify.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly IRepository<DocumentRegister> _documentRegisterRepository;
        private readonly IRepository<EduUniversity> _universityRepository;
        private readonly IMongoRepository<Resource> _resourceRepository;
        private readonly IConfiguration _configuration;
        public DashboardService(IUnitOfWork unitOfWork, IMongoRepository<Resource> resourceRepository, IConfiguration configuration)
        {
            _resourceRepository = resourceRepository;
            _configuration = configuration;
            _notificationRepository = unitOfWork.GetRepository<Notification>();
            _documentRegisterRepository = unitOfWork.GetRepository<DocumentRegister>();
            _universityRepository = unitOfWork.GetRepository<EduUniversity>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _businessRepository = unitOfWork.GetRepository<Business>();
        }


        private LookupModel<int> MentifiTypeLookup(int? eduBusinessType)
        {
            switch (eduBusinessType)
            {
                case 1:
                    return new LookupModel<int>("Mentor", 1);
                case 2:
                    return new LookupModel<int>("Mentee", 2);
                case 3:
                    return new LookupModel<int>("Admin", 3);
                default:
                    return null;
            }
        }

        //TODO:Delete after revert the version
        public async Task<IEnumerable<NotificationModel>> Get(int systemUserId, string url)
        {
            var notifs = _notificationRepository.GetPagedList(predicate: a => a.SystemUserId == systemUserId && (a.Message != "" || a.Message != null), orderBy: o => o.OrderByDescending(a => a.CreatedOn), pageSize: int.MaxValue);
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));
            var university = _universityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));
            var notifSystemusers = notifs.Items.Select(b => b.CreatedBy)
                .ToArray();
            var systemUsers = _systemUserRepository.GetPagedList(predicate: a =>
                notifSystemusers.Contains(a.SystemUserId), include: d => d.Include(b => b.Business), pageSize: int.MaxValue);
            var models = new List<NotificationModel>();
            foreach (var mobileAppNotification in notifs.Items)
            {
                var createdBy = systemUsers.Items.SingleOrDefault(a => a.SystemUserId == mobileAppNotification.CreatedBy);
                var message = await ResourceMessage(mobileAppNotification, createdBy, university, systemUser);
                if (!string.IsNullOrEmpty(message) && createdBy != null)
                    models.Add(new NotificationModel
                    {
                        NotificationId = mobileAppNotification.NotificationId,
                        CreatedOn = mobileAppNotification.CreatedOn.ToUnixTime(),
                        Message = message,
                        IsShowed = mobileAppNotification.IsShowed,
                        NotificationTYpe = mobileAppNotification.NotificationType,
                        Sender = new ProfileModelIncludingEduType
                        {
                            EduBusinessType = MentifiTypeLookup(createdBy.Business.EduBusinessType),
                            FullName = createdBy.FullName,
                            Id = createdBy.SystemUserId,
                            PhotoUrl = createdBy.ToPhotoUrl(baseUrl: url)
                        }
                    });
            }
            return models;
        }

        public async Task<PagedListModel<NotificationModel>> Get(int systemUserId, string url, int limit, int pageIndex)
        {
            var notifs = _notificationRepository
                .GetPagedList(predicate: a => a.SystemUserId == systemUserId && (a.Message != "" || a.Message != null),
                    orderBy: o => o.OrderByDescending(a => a.CreatedOn), pageIndex: pageIndex, pageSize: limit);

            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));
            var university = _universityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));
            var notifSystemusers = notifs.Items.Select(b => b.CreatedBy)
                .ToArray();
            var systemUsers = _systemUserRepository.GetPagedList(predicate: a =>
                notifSystemusers.Contains(a.SystemUserId), include: d => d.Include(b => b.Business));
            var models = new List<NotificationModel>();
            foreach (var mobileAppNotification in notifs.Items)
            {
                var createdBy = systemUsers.Items.FirstOrDefault(a => a.SystemUserId == mobileAppNotification.CreatedBy);
                var message = await ResourceMessage(mobileAppNotification, createdBy, university, systemUser, false);
                if (!string.IsNullOrEmpty(message) && createdBy != null)
                    models.Add(new NotificationModel
                    {
                        NotificationId = mobileAppNotification.NotificationId,
                        CreatedOn = mobileAppNotification.CreatedOn.ToUnixTime(),
                        Message = message,
                        IsShowed = mobileAppNotification.IsShowed,
                        NotificationTYpe = mobileAppNotification.NotificationType,
                        Sender = new ProfileModelIncludingEduType
                        {
                            EduBusinessType = MentifiTypeLookup(createdBy.Business.EduBusinessType),
                            FullName = createdBy.FullName,
                            Id = createdBy.SystemUserId,
                            PhotoUrl = createdBy.ToPhotoUrl(baseUrl: url)
                        }
                    });
            }

            return models.Map(notifs);
        }

        public async Task<string> ResourceMessage(Notification mobileAppNotification, SystemUser createdBy, EduUniversity university, SystemUser systemUser, bool isHtmlVersion = true)
        {
            Resource resource;
            var labelSetCode = "Mentifi";
            var sb = new StringBuilder();
            var mentifiWebUrl = _configuration.GetSection("MentifiWebUrl").ToString();
            string resourceName;
            switch (mobileAppNotification.NotificationType)
            {
                case Constant.NOTIFTYPE_NEXTACTIVITYSEQUENCE:
                    //resource =
                    //(await _resourceRepository.GetAll(a => a.ResourceName == labelSetCode + "_NotifNextActivitySequence")).First();
                    //sb.Append(string.Format(resource.ResourceValue, mentifiWebUrl + "CRM/ShowMyActivity?q=" + SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())) + ";" + mobileAppNotification.CreatedOn.ToString("dd/MM/yyyy HH:mm") + ";" + createdBy?.FullName + ";" + mobileAppNotification.CreatedBy.ToString());
                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has assigned you an <a href='{mentifiWebUrl + "CRM/ShowMyActivity?q=" + SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())}'>activity</a> ready to work on."
                        : $"{createdBy?.FullName} has assigned you an activity ready to work on.";

                case Constant.NOTIFTYPE_ASSIGNMENT:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == labelSetCode + "_NotifAssignment")).First();
                    //sb.Append(string.Format(resource.ResourceValue, mentifiWebUrl + "CRM/ShowMyActivity?q=" + SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())) + ";" + mobileAppNotification.CreatedOn.ToString("dd/MM/yyyy HH:mm") + ";" + createdBy?.FullName + ";" + mobileAppNotification.CreatedBy.ToString());

                    //break;
                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has assigned you a new <a href='{mentifiWebUrl + "CRM/ShowMyActivity?q=" + SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())}'>activity</a>."
                        : $"{createdBy?.FullName} has assigned you a new activity.";

                case Constant.NOTIFTYPE_EDUCONNECT:
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == labelSetCode + "_NotifBusinessJoin")).First();
                    return createdBy?.FullName + " " + resource.ResourceValue;

                case Constant.NOTIFTYPE_BUSINESSLINK:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == labelSetCode + "_NotifMessageBoardLink")).First();
                    //sb.Append(createdBy?.FullName + " " + resource.ResourceValue);
                    //break;
                    return
                        isHtmlVersion
                            ? $"{createdBy?.FullName} has sent you a <a href=\"{mentifiWebUrl}mentifi/network\">connection request</a>, communicate further in message board."
                            : $"{createdBy?.FullName} has sent you a connection request, communicate further in message board.";

                case Constant.NOTIFTYPE_MESSAGE:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == labelSetCode + "_NotifMessage")).First();
                    //return string.Format(resource.ResourceValue, mentifiWebUrl + ");
                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has sent you a <a href='{mentifiWebUrl}Message/ShowMessage?q=\"{SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())}\"'>hub message</a>."
                        : $"{createdBy?.FullName} has sent you a hub message.";

                case Constant.NOTIFTYPE_PRIVATEBULLETIN:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == labelSetCode + "_NotifPrivateBulletin")).First();
                    //return createdBy?.FullName + " " + string.Format(resource.ResourceValue,
                    //           mentifiWebUrl + "Bulletin/ShowBulletin?q=" +
                    //           SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString()));
                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has shared a <a href='{0}'>private bulletin</a> with you."
                        : $"{createdBy?.FullName} has shared a private bulletin with you.";

                case Constant.NOTIFTYPE_PRIVATEBULLETINREPLY:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == labelSetCode + "_NotifPrivateBulletinReply")).First();
                    //return createdBy?.FullName + " " + string.Format(resource.ResourceValue,
                    //           mentifiWebUrl + "Bulletin/ShowBulletin?q=" +
                    //           SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString()));
                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has replied to your <a href='{mentifiWebUrl}Bulletin/ShowBulletin?q=\"{SecurityUtility.Encrypt(mobileAppNotification.RegardingId.ToString())}\"'>private bulletin</a> post."
                        : $"{createdBy?.FullName} has replied to your private bulletin post";

                case Constant.NOTIFTYPE_NEWLYJOINED:
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == labelSetCode + "_NotifNewlyJoined")).First();
                    sb.Append(createdBy?.FullName + resource.ResourceValue);
                    break;

                case Constant.NOTIFTYPE_BUSINESSJOIN:
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == labelSetCode + "_NotifBusinessJoin")).First();
                    sb.Append(createdBy?.FullName + " " + resource.ResourceValue);
                    break;

                case Constant.NOTIFTYPE_LINKACTIVATE:
                    resource =
                        (await _resourceRepository.GetAll(
                            a => a.ResourceName == labelSetCode + "_NotifLinkActivated")).First();
                    return $"{createdBy?.FullName } {resource.ResourceValue}";

                case Constant.NOTIFTYPE_INVITEACTIVATE:
                    resource =
                        (await _resourceRepository.GetAll(
                            a => a.ResourceName == labelSetCode + "_NotifInviteActive")).First();
                    return string.Format(resource.ResourceValue, university);

                case Constant.GlobalNotification.RecommendToConnect:
                case Constant.GlobalNotification.AcceptBusinessConnection:
                    return $"{createdBy?.FullName} {mobileAppNotification.Message}";

                case Constant.MentifiNotification.ADDED_CONNECTION_BY_UNIVERSITY:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == "AddedConnectionWithLink")).First();
                    var businessIdAdded = Convert.ToInt16(mobileAppNotification.Message);
                    var businessAdded = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdAdded, include: a => a.Include(b => b.SystemUser));
                    var userAdded = businessAdded.SystemUser.FirstOrDefault();
                    //return string.Format(resource.ResourceValue, university.UniversityNameAlias, userAdded?.SystemUserId, userAdded?.FullName);
                    return isHtmlVersion
                        ? $"{university.UniversityNameAlias} admin, has added your connection with <b class='btn-show-profile-business' id='notif-name_{userAdded?.SystemUserId}'>{userAdded?.FullName}</b>"
                        : $"{university.UniversityNameAlias} admin, has added your connection with {userAdded?.FullName}";

                case Constant.MentifiNotification.ADDED_CONNECTION_BY_UNIVERSITY_REACHED_THE_LIMIT:
                    var businessIdAddedReachedTheLimit = Convert.ToInt16(mobileAppNotification.Message);
                    var businessAddedReachedTheLimit = _businessRepository.GetFirstOrDefault(
                        predicate: a => a.BusinessId == businessIdAddedReachedTheLimit,
                        include: a => a.Include(b => b.SystemUser));
                    var userAddedReachedTheLimit = businessAddedReachedTheLimit.SystemUser.FirstOrDefault();
                    var userAddedReachedTheLimitAlias = businessAddedReachedTheLimit.EduBusinessType == (int)EduBusinessType.Mentee ? university.MenteeAlias : university.MentorAlias;
                    return isHtmlVersion
                        ? $"{university.UniversityNameAlias} has added your connection with <b class='btn-show-profile-business' id='notif-name_{userAddedReachedTheLimit?.SystemUserId}'>{userAddedReachedTheLimit?.FullName}</b> & you just reached the {userAddedReachedTheLimitAlias} limit. Your remaining connect requests will be removed automatically."
                        : $"{university.UniversityNameAlias} has added your connection with {userAddedReachedTheLimit?.FullName} & you just reached the {userAddedReachedTheLimitAlias} limit. Your remaining connect requests will be removed automatically.";

                case Constant.MentifiNotification.REMOVE_CONNECTION_BY_UNIVERSITY:
                    var businessIdRemoved = Convert.ToInt16(mobileAppNotification.Message);
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == "RemoveConnectionWithLink")).First();
                    var businessRemoved = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdRemoved, include: a => a.Include(b => b.SystemUser));
                    var userRemoved = businessRemoved.SystemUser.FirstOrDefault();
                    //return string.Format(resource.ResourceValue, university.UniversityNameAlias, userRemoved?.SystemUserId, userRemoved?.FullName);
                    return isHtmlVersion
                        ? $"{university.UniversityNameAlias} has removed your connection request from <b class='btn-show-profile-business' id='notif-name_{userRemoved?.SystemUserId}'>{userRemoved?.FullName}</b>"
                        : $"{university.UniversityNameAlias} has removed your connection request from {userRemoved?.FullName}";

                case Constant.MentifiNotification.ACCEPT_CONNECTION_BY_UNIVERSITY:
                    var businessIdAccepted = Convert.ToInt16(mobileAppNotification.Message);
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == "AcceptConnectionWithLink")).First();
                    var businessAccepted = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdAccepted, include: a => a.Include(b => b.SystemUser));
                    var userAccepted = businessAccepted.SystemUser.FirstOrDefault();

                    //return string.Format(resource.ResourceValue, university.UniversityNameAlias, userAccepted?.SystemUserId, userAccepted?.FullName);
                    return isHtmlVersion
                        ? $"{university.UniversityNameAlias} has accepted your connection request from <b class='btn-show-profile-business' id='notif-name_{userAccepted?.SystemUserId}'>{userAccepted?.FullName}</b>"
                        : $"{university.UniversityNameAlias} has accepted your connection request from {userAccepted?.FullName}";

                case Constant.MentifiNotification.ACCEPT_CONNECTION_BY_UNIVERSITY_REACHED_THE_LIMIT:
                    var businessIdAcceptedReachedTheLimit = Convert.ToInt16(mobileAppNotification.Message);

                    var businessAcceptedReachedTheLimit = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdAcceptedReachedTheLimit, include: x => x.Include(y => y.SystemUser));
                    var userAcceptedReachedTheLimit = businessAcceptedReachedTheLimit.SystemUser.FirstOrDefault();

                    var userAcceptedReachedTheLimitAlias = businessAcceptedReachedTheLimit.EduBusinessType == (int)EduBusinessType.Mentee ? university.MenteeAlias : university.MentorAlias;

                    return
                        $"{university.UniversityNameAlias} has accepted  your connection with <b class='btn-show-profile-business' id='notif-name_{userAcceptedReachedTheLimit?.SystemUserId}'>{userAcceptedReachedTheLimit?.FullName}</b> & you just reached the {userAcceptedReachedTheLimitAlias} limit. Your remaining connect requests will be removed automatically.";


                case Constant.MentifiNotification.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_FROM:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == "RemoveConnectionFromWithLink")).First();
                    var businessIdRemovedFrom = Convert.ToInt16(mobileAppNotification.Message);
                    var userRemovedFrom = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdRemovedFrom, include: x => x.Include(y => y.SystemUser))?.SystemUser.FirstOrDefault();
                    //return createdBy?.FullName + " " + string.Format(resource.ResourceValue, userRemovedFrom?.SystemUserId, userRemovedFrom?.FullName);

                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has removed the connection request from <b class='btn-show-profile-business' id='notif-name_{userRemovedFrom?.SystemUserId}'>{userRemovedFrom?.FullName}</b>"
                        : $"{createdBy?.FullName} has removed the connection request from {userRemovedFrom?.FullName}";

                case Constant.MentifiNotification.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_TO:
                    //resource =
                    //(await _resourceRepository.GetAll(
                    //    a => a.ResourceName == "RemoveConnectionToWithLink")).First();
                    var businessIdRemovedTo = Convert.ToInt16(mobileAppNotification.Message);
                    var userRemovedTo = _businessRepository.GetFirstOrDefault(predicate: x => x.BusinessId == businessIdRemovedTo, include: x => x.Include(y => y.SystemUser))?.SystemUser.FirstOrDefault();
                    //return createdBy?.FullName + " " + string.Format(resource.ResourceValue, userRemovedTo?.SystemUserId, userRemovedTo?.FullName);

                    return isHtmlVersion
                        ? $"{createdBy?.FullName} has removed the connection request from <b class='btn-show-profile-business' id='notif-name_{userRemovedTo?.SystemUserId}'>{userRemovedTo?.FullName}</b>"
                        : $"{createdBy?.FullName} has removed the connection request from {userRemovedTo?.FullName}";

                case Constant.NOTIFTYPE_EDUREJECT:
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == labelSetCode + "_NotifLinkRejected")).First();
                    return $"{createdBy?.FullName} {resource.ResourceValue}";

                case Constant.MentifiNotification.GOAL_ADDED:
                    resourceName = isHtmlVersion ? "GoalCreatedWithLink" : "GoalCreated";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has just created a new goal.";

                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();
                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.GOAL_EDITED:
                    resourceName = isHtmlVersion ? "GoalEditedWithLink" : "GoalEdited";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has edited a goal.";

                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();
                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.GOAL_PROGRESS_ADDED:
                    resourceName = isHtmlVersion ? "GoalProgressCreatedWithLink" : "GoalProgressCreated";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has added a progress to his/her goal";
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();
                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.GOAL_REMOVED:
                    resourceName = isHtmlVersion ? "GoalRemovedWithLink" : "GoalRemoved";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has removed a goal";
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();

                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.TASK_CREATED:
                    resourceName = isHtmlVersion ? "TaskCreatedWithLink" : "TaskCreated";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has just added a new task.";
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();

                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.TASK_ASSIGNED:
                    resourceName = isHtmlVersion ? "TaskAssignedWithLink" : "TaskAssigned";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has assigned you a task";
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();

                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.TASK_COMPLETED:
                    resourceName = isHtmlVersion ? "TaskCompletedWithLink" : "TaskCompleted";
                    if (isHtmlVersion)
                        return
                            $"{createdBy?.FullName} has completed a task.";
                    resource =
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == resourceName)).First();

                    return createdBy?.FullName + " " + string.Format(resource.ResourceValue, mobileAppNotification.RegardingId);

                case Constant.MentifiNotification.TASK_CONVERTED_TO_PROJECT:
                    resource =
                    (await _resourceRepository.GetAll(
                        a => a.ResourceName == "TaskConvertedToProject")).First();
                    return createdBy?.FullName + " " + resource.ResourceValue;

                case Constant.NOTIFTYPE_MENTIFIMESSAGEBOARDREQUESTSENT:
                    return "You has sent a message through message board of connection request";

                case Constant.NOTIFTYPE_MENTIFIMESSAGEBOARD:
                    return $"{createdBy?.FullName} has sent you a message through message board of connection request";
            }

            return sb.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetDocumentFolderPath(DocumentRegister document)
        {
            var res = new List<string>();


            if (document.DocumentParentId != 0 && document.DocumentParentId.HasValue)
            {
                GetPath(res, document.DocumentParentId.Value);
            }
            else
            {
                GetPath(res, document.RegardingEntityId ?? 0);
            }

            var sb = new StringBuilder();
            sb.Append(res[res.Count() - 1]);
            for (var i = res.Count() - 2; i >= 0; i += -1)
            {
                sb.AppendFormat("/{0}", res[i]);
            }

            return sb.ToString();
        }

        private void GetPath(List<string> result, int documentId)
        {
            var query = _documentRegisterRepository.GetFirstOrDefault(predicate: a => a.DocumentId == documentId);

            result.Add(query.DocumentName);

            if (query.DocumentParentId != 0 && query.DocumentParentId.HasValue)
            {
                GetPath(result, query.DocumentParentId.Value);
            }
        }
    }
}
