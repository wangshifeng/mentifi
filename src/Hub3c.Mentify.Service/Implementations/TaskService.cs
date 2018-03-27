using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MentifiTask> _taskRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<MentifiChannelTask> _mentifiTaskChannelRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IBusInstance _busInstance;

        public TaskService(IUnitOfWork unitOfWork, IBusInstance busInstance)
        {
            _unitOfWork = unitOfWork;
            _busInstance = busInstance;
            _taskRepository = unitOfWork.GetRepository<MentifiTask>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _mentifiTaskChannelRepository = unitOfWork.GetRepository<MentifiChannelTask>();
            _notificationRepository = unitOfWork.GetRepository<Notification>();
        }

        public IEnumerable<TaskModel> GetPendingByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId, int toSystemUserId, string baseUrl)
        {
            var taskOwner =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId, include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var taskReceiver =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId, include: a => a.Include(b => b.Business));

            ValidateAssigning(taskOwner, taskReceiver);

            var mentor = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentor ? taskOwner : taskReceiver;
            var mentee = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentee ? taskOwner : taskReceiver;
            var channelTask = GetChannelTask(mentor.BusinessId, mentee.BusinessId);

            if (channelTask == null)
                return Enumerable.Empty<TaskModel>();

            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, mentee, mentor),
                predicate: a =>
                    a.MentifiChannelTaskId == channelTask.MentifiChannelTaskId &&
                    a.Status != MentifiTaskStatus.Completed,
                include: a =>
                    a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness)
                        .ThenInclude(c => c.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence), pageSize: int.MaxValue).Items;
        }

        public IEnumerable<TaskModel> GetCompletedByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId, int toSystemUserId, string baseUrl)
        {
            var taskOwner =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId, include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var taskReceiver =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId, include: a => a.Include(b => b.Business));

            ValidateAssigning(taskOwner, taskReceiver);

            var mentor = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentor ? taskOwner : taskReceiver;
            var mentee = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentee ? taskOwner : taskReceiver;
            var channelTask = GetChannelTask(mentor.BusinessId, mentee.BusinessId);

            if (channelTask == null)
                return Enumerable.Empty<TaskModel>();

            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, mentee, mentor), predicate: a => a.MentifiChannelTaskId == channelTask.MentifiChannelTaskId && a.Status == MentifiTaskStatus.Completed,
                include: a => a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness).ThenInclude(c => c.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence)).Items;
        }

        public PagedListModel<TaskModel> GetPendingByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId, int toSystemUserId, string baseUrl, int pageIndex, int limit)
        {
            var taskOwner =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId, include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var taskReceiver =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId, include: a => a.Include(b => b.Business));

            ValidateAssigning(taskOwner, taskReceiver);

            var mentor = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentor ? taskOwner : taskReceiver;
            var mentee = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentee ? taskOwner : taskReceiver;
            var channelTask = GetChannelTask(mentor.BusinessId, mentee.BusinessId);

            if (channelTask == null)
                return null;

            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, mentee, mentor),
                predicate: a =>
                    a.MentifiChannelTaskId == channelTask.MentifiChannelTaskId &&
                    a.Status != MentifiTaskStatus.Completed,
                include: a =>
                    a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness)
                        .ThenInclude(c => c.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence), pageIndex: pageIndex, pageSize: limit).Map();
        }

        public IEnumerable<TaskModel> GetMyTask(int systemUserId, string baseUrl, bool isCompleted)
        {
            var business = _systemUserRepository.GetFirstOrDefault(selector: a => a.Business, predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));
            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, null, null),
                predicate: a => a.AssignedTo == business.BusinessId && (isCompleted
                                    ? a.Status == MentifiTaskStatus.Completed
                                    : a.Status != MentifiTaskStatus.Completed),
                pageSize: int.MaxValue,
                include: a => a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness).ThenInclude(c => c.SystemUser).Include(b => b.MentifiChannelTask).ThenInclude(c => c.Mentor).ThenInclude(d => d.SystemUser).Include(b => b.MentifiChannelTask).ThenInclude(c => c.Mentee).ThenInclude(d => d.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence)).Items;
        }

        public PagedListModel<TaskModel> GetCompletedByAssignedToSystemUserIdAndSystemUserId(int fromSystemUserId, int toSystemUserId, string baseUrl, int pageIndex, int limit)
        {
            var taskOwner =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId, include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var taskReceiver =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId, include: a => a.Include(b => b.Business));

            ValidateAssigning(taskOwner, taskReceiver);

            var mentor = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentor ? taskOwner : taskReceiver;
            var mentee = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentee ? taskOwner : taskReceiver;
            var channelTask = GetChannelTask(mentor.BusinessId, mentee.BusinessId);

            if (channelTask == null)
                return null;

            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, mentee, mentor),
                predicate: a =>
                    a.MentifiChannelTaskId == channelTask.MentifiChannelTaskId &&
                    a.Status == MentifiTaskStatus.Completed,
                include: a =>
                    a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness)
                        .ThenInclude(c => c.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence), pageIndex: pageIndex, pageSize: limit).Map();
        }


        public PagedListModel<TaskModel> GetMyTask(int systemUserId, string baseUrl, int pageIndex, int limit, bool isCompleted)
        {
            var business = _systemUserRepository.GetFirstOrDefault(selector: a => a.Business,
                predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));

            return _taskRepository.GetPagedList(selector: a => Mapping(a, baseUrl, null, null),
                predicate: a => a.AssignedTo == business.BusinessId && (isCompleted
                                    ? a.Status == MentifiTaskStatus.Completed
                                    : a.Status != MentifiTaskStatus.Completed),
                include: a => a.Include(d => d.CreatedByBusiness).Include(b => b.AssignedToBusiness)
                    .ThenInclude(c => c.SystemUser).Include(b => b.MentifiChannelTask).ThenInclude(c => c.Mentor)
                    .ThenInclude(d => d.SystemUser).Include(b => b.MentifiChannelTask).ThenInclude(c => c.Mentee)
                    .ThenInclude(d => d.SystemUser),
                orderBy: a => a.OrderBy(b => b.Sequence), pageIndex: pageIndex, pageSize: limit).Map();
        }

        public void Create(NewTaskModel model)
        {
            var taskOwner =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.FromSystemUserId,
                    include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var taskReceiver =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.ToSystemUserId,
                    include: a => a.Include(b => b.Business));

            var assigneeSystemUser =
                model.AssignedToSystemUserId.HasValue
                    ? _systemUserRepository.GetFirstOrDefault(
                        predicate: a => a.SystemUserId == model.AssignedToSystemUserId,
                        include: a => a.Include(b => b.Business))
                    : null;

            ValidateAssigning(taskOwner, taskReceiver);

            var mentor = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentor ? taskOwner : taskReceiver;
            var mentee = taskOwner.Business.EduBusinessType == (int)EduBusinessType.Mentee ? taskOwner : taskReceiver;
            var channelTask = GetChannelTask(mentor.BusinessId, mentee.BusinessId);
            var lastSequence = channelTask != null
                ? _taskRepository.GetFirstOrDefault(predicate: a => a.MentifiTaskId == channelTask.MentifiChannelTaskId,
                      orderBy: a => a.OrderByDescending(b => b.Sequence))?.Sequence ?? 0
                : 0;

            var task = new MentifiTask()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = taskOwner.BusinessId,
                Status = model.Status,
                Priority = model.Priority,
                AssignedTo = assigneeSystemUser?.BusinessId,
                DueDate = model.DueDate.FromUnixTime(),
                TaskSubject = model.Subject,
                Sequence = lastSequence,
                MentifiChannelTaskId = channelTask?.MentifiChannelTaskId ?? 0
            };

            if (channelTask == null)
            {
                task.MentifiChannelTask = new MentifiChannelTask()
                {
                    MenteeId = mentee.BusinessId,
                    MentorId = mentor.BusinessId
                };
            }
            _taskRepository.Insert(task);

            CreateNotif(taskOwner, Constant.MentifiNotification.TASK_CREATED, taskReceiver.SystemUserId);

            if (model.AssignedToSystemUserId.HasValue && model.AssignedToSystemUserId > 0)
                CreateNotif(taskOwner, Constant.MentifiNotification.TASK_ASSIGNED, model.AssignedToSystemUserId.Value);

            if (model.Status == MentifiTaskStatus.Completed)
                CreateNotif(taskOwner, Constant.MentifiNotification.TASK_COMPLETED, taskReceiver.SystemUserId);

            _unitOfWork.SaveChanges();
        }

        public void Edit(EditedTaskModel model)
        {
            var task = _taskRepository.GetFirstOrDefault(predicate: a => a.MentifiTaskId == model.Id,
                include: a =>
                    a.Include(b => b.MentifiChannelTask).Include(b => b.AssignedToBusiness)
                        .ThenInclude(c => c.SystemUser));
            if (task == null)
                throw new ApplicationException("The task id is invalid");

            var mentor =
                _systemUserRepository.GetFirstOrDefault(
                    predicate: a => a.BusinessId == task.MentifiChannelTask.MentorId);
            var mentee =
                _systemUserRepository.GetFirstOrDefault(
                    predicate: a => a.BusinessId == task.MentifiChannelTask.MenteeId);

            var modifiedBySystemUser = model.ModifiedBySystemUserId == mentee.SystemUserId ? mentee : mentor;

            var assigneeSystemUser =
                model.AssignedToSystemUserId.HasValue
                    ? _systemUserRepository.GetFirstOrDefault(
                        predicate: a => a.SystemUserId == model.AssignedToSystemUserId,
                        include: a => a.Include(b => b.Business))
                    : null;

            if (assigneeSystemUser != null && model.AssignedToSystemUserId > 0 && task.AssignedTo != assigneeSystemUser.BusinessId)
                CreateNotif(modifiedBySystemUser, Constant.MentifiNotification.TASK_ASSIGNED, assigneeSystemUser.SystemUserId);

            if (model.Status == MentifiTaskStatus.Completed)
                CreateNotif(modifiedBySystemUser, Constant.MentifiNotification.TASK_COMPLETED,
                    modifiedBySystemUser.SystemUserId == mentee.SystemUserId
                        ? mentee.SystemUserId
                        : mentor.SystemUserId);

            task.AssignedToBusiness = assigneeSystemUser?.Business;
            task.AssignedTo = assigneeSystemUser?.BusinessId;
            task.DueDate = model.DueDate.FromUnixTime();
            task.Priority = model.Priority;
            task.ModifiedOn = DateTime.UtcNow;
            task.ModifiedBy = model.ModifiedBySystemUserId;
            task.TaskSubject = model.Subject;
            task.Status = model.Status;

            _taskRepository.Update(task);
            _unitOfWork.SaveChanges();

        }

        public void Assign(TaskAssigneeModel model)
        {
            var task = _taskRepository.GetFirstOrDefault(predicate: a => a.MentifiTaskId == model.Id);
            if (task == null)
                throw new ApplicationException("The task id is invalid");

            var assigneeSystemUser = model.AssignedToSystemUserId.HasValue ?
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.AssignedToSystemUserId) : null;

            var modifiedSystemUSer =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.ModifiedBySystemUserId);

            if (assigneeSystemUser != null && model.AssignedToSystemUserId > 0 && task.AssignedTo != assigneeSystemUser.BusinessId)
                CreateNotif(modifiedSystemUSer, Constant.MentifiNotification.TASK_ASSIGNED, model.AssignedToSystemUserId.Value);

            task.AssignedTo = assigneeSystemUser?.BusinessId;
            task.ModifiedOn = DateTime.UtcNow;
            task.ModifiedBy = modifiedSystemUSer.BusinessId;
            _taskRepository.Update(task);
            _unitOfWork.SaveChanges();
        }

        public void Delete(int taskId)
        {
            var task = _taskRepository.GetFirstOrDefault(predicate: a => a.MentifiTaskId == taskId);
            if (task == null) throw new ApplicationException("The task id is invalid");

            _taskRepository.Delete(task);
            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(TaskStatusModel model)
        {
            var task = _taskRepository.GetFirstOrDefault(predicate: a => a.MentifiTaskId == model.Id,
                include: a => a.Include(b => b.MentifiChannelTask));

            if (task == null) throw new ApplicationException("The task id is invalid");

            var modifiedSystemUSer =
                _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.ModifiedBySystemUserId);

            task.Status = model.Status;
            task.ModifiedBy = modifiedSystemUSer.BusinessId;
            task.ModifiedOn = DateTime.UtcNow;
            _taskRepository.Update(task);

            if (model.Status == MentifiTaskStatus.Completed)
            {
                CreateNotif(modifiedSystemUSer, Constant.MentifiNotification.TASK_COMPLETED,
                    modifiedSystemUSer.SystemUserId == task.MentifiChannelTask.MenteeId
                        ? task.MentifiChannelTask.MentorId
                        : task.MentifiChannelTask.MenteeId);
            }

            _unitOfWork.SaveChanges();
        }


        private MentifiChannelTask GetChannelTask(int mentorBusinessId, int menteeBusinessId)
        {
            return _mentifiTaskChannelRepository.GetFirstOrDefault(predicate: a =>
                a.MenteeId == menteeBusinessId && a.MentorId == mentorBusinessId);
        }

        private void ValidateAssigning(SystemUser taskOwner, SystemUser taskReceiver)
        {

            if (taskOwner.Business.EduBusinessType == taskReceiver.Business.EduBusinessType)
                throw new ApplicationException("The owner task and receiver has same edu business type");

            if (!taskOwner.Business.BusinessToBusiness.Any(a =>
                a.BusinessId2 == taskReceiver.BusinessId && a.IsActive == true))
                throw new ApplicationException("The owner task and receiver has no active connection");

        }

        private void CreateNotif(SystemUser fromSystemUser, string notifType, int toSystemUserId)
        {

            var model = new Notification
            {
                SystemUserId = toSystemUserId,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = fromSystemUser.SystemUserId,
                SystemUserType = Constant.USERTYPE_ADVISER,
                NotificationType = notifType,
                IsShowed = false,
                RegardingId = fromSystemUser.BusinessId,
                Message = string.Empty
            };

            _notificationRepository.Insert(model);

            _busInstance.Publish(new NotificationAdded
            {
                NotificationType = notifType,
                SystemUserID = toSystemUserId,
                SystemUserType = Constant.USERTYPE_ADVISER,
                CreatedOn = DateTime.UtcNow,
                IsShowed = false,
                CreatedBy = fromSystemUser.SystemUserId,
            });
        }

        private TaskModel Mapping(MentifiTask task, string baseUrl, SystemUser mentee, SystemUser mentor)
        {
            var status = Enum.GetName(typeof(MentifiTaskStatus), task.Status);
            var priority = Enum.GetName(typeof(MentifiTaskPriority), task.Priority);
            var assignedTo = task.AssignedToBusiness?.SystemUser.FirstOrDefault();

            return new TaskModel
            {
                Id = task.MentifiTaskId,
                Subject = task.TaskSubject,
                Status = new LookupModel<int>(status, (int)task.Status),
                Priority = new LookupModel<int>(priority, (int)task.Priority),
                Mentor = mentor != null ? mentor.ToProfileModel(baseUrl) : task.MentifiChannelTask?.Mentor?.SystemUser.SingleOrDefault()?.ToProfileModel(baseUrl),
                Mentee = mentee != null ? mentee.ToProfileModel(baseUrl) : task.MentifiChannelTask?.Mentee?.SystemUser.SingleOrDefault()?.ToProfileModel(baseUrl),
                AssignedTo = assignedTo?.ToProfileModel(baseUrl),
                DueDate = task.DueDate.ToUnixTime(),
                Sequence = task.Sequence,
            };
        }
    }
}
