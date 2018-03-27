using System;
using System.Collections.Generic;
using System.Linq;
using Hub3c.ApiMessage;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class GoalService : IGoalService
    {
        public GoalService(IUnitOfWork unitOfWork,
            IBusInstance busInstance)
        {
            _unitOfWork = unitOfWork;
            _busInstance = busInstance;
            _goalRepository = unitOfWork.GetRepository<MentifiGoal>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _notificationRepository = unitOfWork.GetRepository<Notification>();
            _businessToBusinessRepository = unitOfWork.GetRepository<BusinessToBusiness>();
            _goalProgressRepository = unitOfWork.GetRepository<MentifiGoalProgress>();
        }

        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<MentifiGoal> _goalRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IBusInstance _busInstance;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<MentifiGoalProgress> _goalProgressRepository;

        public IEnumerable<GoalModel> GetBySystemUser(int systemUserId)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId);
            if (systemUser == null)
                throw new ApplicationException("System User Id is invalid.");

            var goals = _goalRepository.GetPagedList(predicate: a => a.CreatedBy == systemUser.BusinessId,
                include: a => a.Include(b => b.MentifiGoalProgress), pageSize: int.MaxValue).Items;

            return (from goal in goals
                    select new GoalModel
                    {
                        Id = goal.MentifiGoalId,
                        Description = goal.GoalDescription,
                        Probability = new LookupModel<int>(Probability(goal.Probability), (int)goal.Probability),
                        ProgressHistories = goal.MentifiGoalProgress?.OrderBy(b => b.CreatedOn)
                            .Select(c => new GoalProgressModel
                            {
                                Id = c.MentifiGoalProgressId,
                                ProgressValue = c.ProgressPercentage,
                                CreatedOn = c.CreatedOn.ToUnixTime()
                            })
                    }).ToList();
        }

        private string Probability(MentifiGoalProbability probability)
        {
            switch (probability)
            {
                case MentifiGoalProbability.SuperHard:
                    return "Super Hard";
                default:
                    return Enum.GetName(typeof(MentifiGoalProbability), probability);
            }
        }

        private void CheckUserIsMentee(Business menteeBusiness)
        {
            if (menteeBusiness.EduBusinessType != (int)EduBusinessType.Mentee) throw new ApplicationException("The User is not mentee.");
        }

        private void ValidateGoalLimit(int businessId)
        {
            var totalGoals = _goalRepository.Count(a => a.CreatedBy == businessId);
            if (totalGoals >= 50) throw new ApplicationException("You have reached maximum capacity of three goals. Please remove one before you can add a new goal.");

        }

        public void Edit(EditedGoalModel model)
        {
            var mentee = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == model.MenteeSystemUserId, include: a => a.Include(b => b.Business));
            CheckUserIsMentee(mentee.Business);
            var goal = _goalRepository.GetFirstOrDefault(predicate: a => a.MentifiGoalId == model.Id);

            if (goal == null)
                throw new ApplicationException("The Goal Id is invalid");

            goal.GoalDescription = model.Description;
            goal.Probability = (MentifiGoalProbability)model.ProbabilityId;
            goal.Version = ++goal.Version;
            goal.ModifiedOn = DateTime.UtcNow;
            goal.ModifiedBy = model.MenteeSystemUserId;

            CreateNotif(mentee, Constant.MentifiNotification.GOAL_EDITED);
            _goalRepository.Update(goal);

            _unitOfWork.SaveChanges();
        }

        public void Delete(int goalId, int systemUserId)
        {
            var mentee = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));

            var goal = _goalRepository.GetFirstOrDefault(predicate: a => a.MentifiGoalId == goalId, include: b => b.Include(c => c.MentifiGoalProgress));

            if (goal == null)
                throw new ApplicationException("The Goal Id is invalid");

            _goalRepository.Delete(goal);
            DeleteGoalProgress(goal.MentifiGoalProgress);

            CreateNotif(mentee, Constant.MentifiNotification.GOAL_REMOVED);

            _unitOfWork.SaveChanges();
        }

        private void DeleteGoalProgress(IEnumerable<MentifiGoalProgress> goalProgresses)
        {
            foreach (var progress in goalProgresses)
            {
                _goalProgressRepository.Delete(progress);
            }
        }

        public void Create(NewGoalModel model)
        {
            var mentee = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == model.MenteeSystemUserId, include: a => a.Include(b => b.Business));
            CheckUserIsMentee(mentee.Business);
            ValidateGoalLimit(mentee.BusinessId);

            var goal = new MentifiGoal()
            {
                CreatedBy = mentee.BusinessId,
                CreatedOn = DateTime.UtcNow,
                GoalDescription = model.Description,
                Probability = (MentifiGoalProbability)model.ProbabilityId,
                Version = 1,
                MentifiGoalProgress = new List<MentifiGoalProgress>()
                {
                    new MentifiGoalProgress()
                    {
                        CreatedBy = mentee.BusinessId,
                        CreatedOn = DateTime.UtcNow,
                        Version = 1,
                        ProgressPercentage = 0,
                        Reason = "Initial Progress",
                    }
                }
            };
            CreateNotif(mentee, Constant.MentifiNotification.GOAL_ADDED);
            _goalRepository.Insert(goal);
            _unitOfWork.SaveChanges();
        }


        private void CreateNotif(SystemUser createdSystemUser, string notifType)
        {

            var mentors = _businessToBusinessRepository.GetPagedList(
                predicate: a =>
                    a.BusinessId2 == createdSystemUser.BusinessId && a.IsActive == true &&
                    a.Business1.EduBusinessType != (int) EduBusinessType.Admin, pageSize: int.MaxValue,
                include: a => a.Include(b => b.Business1).ThenInclude(b => b.SystemUser)).Items;

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
                        RegardingId = createdSystemUser.SystemUserId
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
                }

            }

        }
    }
}
