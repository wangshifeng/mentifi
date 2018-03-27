using System;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.MongoRepo;
using Hub3c.Mentify.MongoRepo.Model;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hub3c.Mentify.Service.Implementations
{
    public class GoalProgressService : IGoalProgressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MentifiGoalProgress> _goalProgressRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IBusInstance _busInstance;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<EduUniversity> _universityRepository;
        private readonly IRepository<MentifiGoal> _goalRepository;
        private readonly IMongoRepository<Resource> _resourceRepository;

        private readonly IEmailApi _emailApi;
        private readonly IConfiguration _configuration;
        public GoalProgressService(IUnitOfWork unitOfWork, IBusInstance busInstance, IEmailApi emailApi, IConfiguration configuration, IMongoRepository<Resource> resourceRepository)
        {
            _unitOfWork = unitOfWork;
            _busInstance = busInstance;
            _emailApi = emailApi;
            _configuration = configuration;
            _resourceRepository = resourceRepository;
            _goalProgressRepository = _unitOfWork.GetRepository<MentifiGoalProgress>();
            _notificationRepository = _unitOfWork.GetRepository<Notification>();
            _systemUserRepository = _unitOfWork.GetRepository<SystemUser>();
            _businessToBusinessRepository = _unitOfWork.GetRepository<BusinessToBusiness>();
            _universityRepository = _unitOfWork.GetRepository<EduUniversity>();
            _goalRepository = _unitOfWork.GetRepository<MentifiGoal>();
        }

        public void Create(NewGoalProgressModel model)
        {
            var mentee = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == model.MenteeSystemUserId, include: a => a.Include(b => b.Business));
            var goalProgress = new MentifiGoalProgress()
            {
                CreatedBy = mentee.BusinessId,
                CreatedOn = DateTime.UtcNow,
                MentifiGoalId = model.GoalId,
                ProgressPercentage = model.ProgressValue,
                Reason = model.Reason,
                Version = 1
            };
            _goalProgressRepository.Insert(goalProgress);
            CreateNotif(mentee, Constant.MentifiNotification.GOAL_PROGRESS_ADDED, model.GoalId, goalProgress);
            _unitOfWork.SaveChanges();

        }


        private void CreateNotif(SystemUser createdSystemUser, string notifType, int goalId, MentifiGoalProgress goalProgress)
        {

            var mentors = _businessToBusinessRepository.GetPagedList(
                predicate: a =>
                    a.BusinessId2 == createdSystemUser.BusinessId && a.IsActive == true &&
                    a.Business1.EduBusinessType != (int) EduBusinessType.Admin,
                include: a => a.Include(b => b.Business1).ThenInclude(b => b.SystemUser), pageSize: int.MaxValue).Items;
            MentifiGoalProgress lastGoalProgress = null;
            EduUniversity eduUniversity = null;
            Resource resource = null;
            var goal = _goalRepository.GetFirstOrDefault(predicate: a => a.MentifiGoalId == goalId,
                include: a => a.Include(b => b.MentifiGoalProgress));
            if (goal != null)
            {
                lastGoalProgress = goal.MentifiGoalProgress.OrderByDescending(a => a.MentifiGoalProgressId)
                    .FirstOrDefault();
                eduUniversity = _universityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (createdSystemUser.Business.UniversityId ?? createdSystemUser.BusinessId));
                resource =
                  _resourceRepository.GetAll(
                     a => a.ResourceName == "Mentifi_Message_GoalProgressNotification_Subject").GetAwaiter().GetResult().FirstOrDefault();
            }
            foreach (var mentor in mentors)
            {
                var mentorSystemUser = mentor.Business1.SystemUser.FirstOrDefault();
                if (mentorSystemUser != null)
                {
                    var model = new Notification
                    {
                        SystemUserId = mentorSystemUser.SystemUserId,
                        CreatedOn = DateTime.Now,
                        CreatedBy = createdSystemUser.SystemUserId,
                        SystemUserType = Constant.USERTYPE_ADVISER,
                        NotificationType = notifType,
                        IsShowed = false,
                        RegardingId = createdSystemUser.SystemUserId,
                        Message = string.Empty
                    };
                    _notificationRepository.Insert(model);

                    _busInstance.Publish(new NotificationAdded
                    {
                        NotificationType = notifType,
                        SystemUserID = mentorSystemUser.SystemUserId,
                        SystemUserType = Constant.USERTYPE_ADVISER,
                        CreatedOn = DateTime.UtcNow,
                        IsShowed = false,
                        CreatedBy = createdSystemUser.SystemUserId,
                    });

                    if (goal != null)
                    {
                        _emailApi.AddGoalProgress(new EmailParam()
                        {
                            Recipient = new[]
                            {
                                mentorSystemUser.EmailAddress
                            },
                            SystemUserId = mentorSystemUser.SystemUserId,
                            Payload = new[]
                            {
                                mentorSystemUser.FullName,
                                createdSystemUser.FullName,
                                goal.GoalDescription,
                                lastGoalProgress?.ProgressPercentage.ToString(),
                                goalProgress?.ProgressPercentage.ToString(),
                                goalProgress?.Reason,
                                _configuration["MentifiWebUrl"],
                                eduUniversity.UniversityNameAlias,
                                resource?.ResourceValue,
                                eduUniversity.MenteeAlias
                            }
                        }).GetAwaiter().GetResult();
                    }


                }

            }

        }
    }
}
