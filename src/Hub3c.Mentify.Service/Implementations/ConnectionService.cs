using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hub3c.ApiMessage;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Hub3c.Messaging.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Business = Hub3c.Mentify.Repository.Models.Business;
using Constant = Hub3c.Mentify.Service.Models.Constant;

namespace Hub3c.Mentify.Service.Implementations
{
    public class ConnectionService : IConnectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<MentifiApplicationUsage> _mentifiApplicationUsageRepository;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<EduUniversity> _eduUniversityRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly ILookupService _lookupService;
        private readonly IEmailApi _emailApi;
        private readonly IConfiguration _configuration;
        private readonly IBusInstance _busInstance;
        private readonly IMessageBoardService _messageBoardService;
        public ConnectionService(
            IUnitOfWork unitOfWork,
            ILookupService lookupService,
            IEmailApi emailApi,
            IConfiguration configuration,
            IBusInstance busInstance, IMessageBoardService messageBoardService)
        {
            _unitOfWork = unitOfWork;
            _lookupService = lookupService;
            _emailApi = emailApi;
            _configuration = configuration;
            _busInstance = busInstance;
            _messageBoardService = messageBoardService;
            _systemUserRepository = _unitOfWork.GetRepository<SystemUser>();
            _businessToBusinessRepository = _unitOfWork.GetRepository<BusinessToBusiness>();
            _mentifiApplicationUsageRepository = _unitOfWork.GetRepository<MentifiApplicationUsage>();
            _notificationRepository = _unitOfWork.GetRepository<Notification>();
            _eduUniversityRepository = _unitOfWork.GetRepository<EduUniversity>();
            _businessRepository = _unitOfWork.GetRepository<Business>();
        }

        public IEnumerable<ConnectionModel> GetMentor(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(
                 predicate: a => a.SystemUserId == systemUserId,
                 include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (systemUser == null) throw new ApplicationException("System User Not Found");
            var models = new List<ConnectionModel>();

            if (systemUser.Business.EduBusinessType == 0) throw new ApplicationException("University is not found.");
            {
                var userSetting = _eduUniversityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));

                if (systemUser.Business.EduBusinessType != (int)EduBusinessType.Admin)
                {
                    var admin = _businessRepository.GetFirstOrDefault(predicate: a =>
                        a.BusinessId == systemUser.Business.UniversityId.Value, include: b => b.Include(c => c.BusinessToBusiness));
                    var connectedBusinesses = systemUser.Business.BusinessToBusiness;
                    var connectedBusinessIds = connectedBusinesses.Select(a => a.BusinessId2).ToArray();
                    var pendingBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected == false && a.CreatedBy == systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var pendingAndRejectedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected && a.CreatedBy == systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var requestedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected == false && a.CreatedBy != systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var requestedAndRejectedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected && a.CreatedBy != systemUserId)
                        .Select(a => a.BusinessId2).ToArray();

                    var acceptedBusinessIds = connectedBusinesses.Where(a => a.IsActive == true)
                        .Select(a => a.BusinessId2).ToArray();

                    if (admin != null)
                    {
                        var unconnecteds = admin.BusinessToBusiness.Where(a => a.BusinessId2 != null && !connectedBusinessIds.Contains(a.BusinessId2));
                        var unconnectedIds = unconnecteds.Select(a => a.BusinessId2).ToArray();
                        var unconnectedBusinesses = GetBusinessByIds(MentifyRole.Mentor, unconnectedIds);
                        var pendingBusinesses = GetBusinessByIds(MentifyRole.Mentor, pendingBusinessIds);
                        var requestedBusinesses = GetBusinessByIds(MentifyRole.Mentor, requestedBusinessIds);
                        var requestedAndRejecteddBusinesses = GetBusinessByIds(MentifyRole.Mentor, requestedAndRejectedBusinessIds);
                        var pendingAndRejecteddBusinesses = GetBusinessByIds(MentifyRole.Mentor, pendingAndRejectedBusinessIds);
                        var acceptedBusiness = GetBusinessByIds(MentifyRole.Mentor, acceptedBusinessIds);
                        var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject).ToList();
                        models.AddRange(Map(unconnectedBusinesses, lookups, url, "Available", userSetting));
                        models.AddRange(Map(pendingBusinesses, lookups, url, "Pending", userSetting));
                        models.AddRange(Map(pendingAndRejecteddBusinesses, lookups, url, "Pending", userSetting, true));
                        models.AddRange(Map(requestedBusinesses, lookups, url, "Request", userSetting));
                        models.AddRange(Map(requestedAndRejecteddBusinesses, lookups, url, "Request", userSetting, true));
                        models.AddRange(Map(acceptedBusiness, lookups, url, "Connected", userSetting));
                    }
                }
                else
                {
                    var connectedBusinessIds = systemUser.Business.BusinessToBusiness.Select(a => a.BusinessId2).ToArray();
                    var connectedBusinesses = GetBusinessByIds(MentifyRole.Mentor, connectedBusinessIds);
                    var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject).ToList();
                    models.AddRange(Map(connectedBusinesses, lookups, url, "Connected", userSetting));
                }
                return models;
            }
        }

        public IEnumerable<ConnectionModel> GetMentee(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (systemUser == null) throw new ApplicationException("System User Not Found");
            var models = new List<ConnectionModel>();

            if (systemUser.Business.EduBusinessType == 0) throw new ApplicationException("User is not mentifi user");
            {
                var userSetting = _eduUniversityRepository.GetFirstOrDefault(predicate: a =>
                    a.BusinessId == (systemUser.Business.UniversityId ?? systemUser.BusinessId));

                if (systemUser.Business.EduBusinessType != (int)EduBusinessType.Admin)
                {
                    var admin = _businessRepository.GetFirstOrDefault(predicate: a =>
                       a.BusinessId == systemUser.Business.UniversityId.Value, include: b => b.Include(c => c.BusinessToBusiness));
                    var connectedBusinesses = systemUser.Business.BusinessToBusiness;
                    var connectedBusinessIds = connectedBusinesses.Select(a => a.BusinessId2).ToArray();
                    var pendingBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected == false && a.CreatedBy == systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var pendingAndRejectedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected && a.CreatedBy == systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var requestedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected == false && a.CreatedBy != systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var requestedAndRejectedBusinessIds = connectedBusinesses
                        .Where(a => a.IsPending == true && a.IsRejected && a.CreatedBy != systemUserId)
                        .Select(a => a.BusinessId2).ToArray();
                    var acceptedBusinessIds = connectedBusinesses.Where(a => a.IsActive == true)
                        .Select(a => a.BusinessId2).ToArray();
                    if (admin == null) return models;
                    {
                        var unconnecteds = admin.BusinessToBusiness.Where(a =>
                            a.BusinessId2 != null && !connectedBusinessIds.Contains(a.BusinessId2));
                        var unconnectedIds = unconnecteds.Select(a => a.BusinessId2).ToArray();
                        var unconnectedBusinesses = GetBusinessByIds(MentifyRole.Mentee, unconnectedIds);
                        var pendingBusinesses = GetBusinessByIds(MentifyRole.Mentee, pendingBusinessIds);
                        var acceptedBusiness = GetBusinessByIds(MentifyRole.Mentee, acceptedBusinessIds);
                        var requestedBusiness = GetBusinessByIds(MentifyRole.Mentee, requestedBusinessIds);
                        var requestedAndRejectedBusiness =
                            GetBusinessByIds(MentifyRole.Mentee, requestedAndRejectedBusinessIds);
                        var pendingAndRejectedBusiness =
                            GetBusinessByIds(MentifyRole.Mentee, pendingAndRejectedBusinessIds);
                        var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject).ToList();
                        models.AddRange(Map(unconnectedBusinesses, lookups, url, "Available", userSetting));
                        models.AddRange(Map(pendingBusinesses, lookups, url, "Pending", userSetting));
                        models.AddRange(Map(pendingAndRejectedBusiness, lookups, url, "Pending", userSetting, true));
                        models.AddRange(Map(requestedAndRejectedBusiness, lookups, url, "Request", userSetting, true));
                        models.AddRange(Map(requestedBusiness, lookups, url, "Request", userSetting));
                        models.AddRange(Map(acceptedBusiness, lookups, url, "Connected", userSetting));
                    }
                }
                else
                {
                    var connectedBusinessIds =
                        systemUser.Business.BusinessToBusiness.Select(a => a.BusinessId2).ToArray();
                    var connectedBusinesses = GetBusinessByIds(MentifyRole.Mentee, connectedBusinessIds);
                    var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduSubject).ToList();
                    models.AddRange(Map(connectedBusinesses, lookups, url, "Connected", userSetting));
                }
                return models;
            }
        }

        private IEnumerable<Business> GetBusinessByIds(MentifyRole role, params int?[] businessIds)
        {
            return _businessRepository.GetPagedList(
                predicate: a => businessIds.Contains(a.BusinessId) && a.EduBusinessType == (int)role, pageSize: int.MaxValue,
                include: a =>
                    a.Include(b => b.SystemUser).ThenInclude(c => c.EduUser).Include(b => b.SystemUser)
                        .ThenInclude(d => d.EduSubjectPreference).Include(b => b.SystemUser)
                        .ThenInclude(d => d.Business).ThenInclude(e => e.BusinessToBusiness)).Items;

        }

        private IEnumerable<ConnectionModel> Map(IEnumerable<Business> businesess, IEnumerable<LookupModel<int>> lookups, string url, string flag, EduUniversity userSetting, bool isRejected = false)
        {
            var models = new List<ConnectionModel>();
            foreach (var business in businesess)
            {
                var businessSystemUser = business.SystemUser.SingleOrDefault();
                var businessEduUser = businessSystemUser?.EduUser.SingleOrDefault();
                if (businessSystemUser != null && businessEduUser != null && !businessEduUser.IsBlocked && !businessEduUser.IsHidden)
                {
                    models.Add(new ConnectionModel
                    {
                        SystemUserId = businessSystemUser.SystemUserId,
                        LastName = businessSystemUser.LastName,
                        FirstName = businessSystemUser.FirstName,
                        MiddleName = businessSystemUser.MiddleName,
                        PreferredName = businessEduUser.PreferredName,
                        SubjectPreferences = businessSystemUser.EduSubjectPreference?.Select(a => new LookupModel()
                        {
                            FieldOfStudyId = a.FieldOfStudyId,
                            FieldOfStudy = lookups.FirstOrDefault(b => b.Id == a.FieldOfStudyId)?.Name,
                            SubjectExperienceId = a.SubjectPreferenceId,
                            OtherFieldOfStudy = a.OtherFieldOfStudy
                        }),
                        Hobby = businessEduUser.Hobby,
                        WorkPhone = businessSystemUser.WorkPhone,
                        MobilePhone = businessSystemUser.MobilePhone,
                        Email = businessSystemUser.EmailAddress,
                        ProfilePhoto = businessSystemUser.ToPhotoUrl(url),
                        BusinessId = business.BusinessId,
                        Flag = flag,
                        IsRejected = isRejected,
                        Salutation = Salutation(businessSystemUser.Title),
                        IsAbleToConnect = IsAbleToConnect(business, userSetting),
                        SkillGoal = businessEduUser.SkillGoal
                    });
                }


            }
            return models;
        }

        private bool IsAbleToConnect(Business business, EduUniversity userSetting)
        {
            var connected =
                business.BusinessToBusiness.Count(a => a.IsActive == true && a.BusinessId2 != userSetting.BusinessId);
            int maxConnection;
            switch (business.EduBusinessType)
            {
                case (int)EduBusinessType.Mentor:
                    maxConnection = userSetting.MaxNumberMenteeForMentor;
                    break;
                case (int)EduBusinessType.Mentee:
                    maxConnection = userSetting.MaxNumberMentorForMentee;
                    break;
                case (int)EduBusinessType.Admin:
                    return true;
                default:
                    return false;
            }
            return connected <= maxConnection;
        }

        /// <summary>
        /// friend of current user
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerable<ConnectionModel> GetConnected(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (systemUser == null) throw new ApplicationException("System User is not found ");

            var business = systemUser.Business;
            if (business == null) throw new ApplicationException("Business is not found ");
            if (business.EduBusinessType == 0) throw new ApplicationException("User is not mentifi user");

            var businessToBusinesses = systemUser.Business.BusinessToBusiness.Where(a => a.IsActive == true)
                .Select(b => b.BusinessId2).ToArray();
            // ReSharper disable once InconsistentNaming
            var businessId2s =
                _businessRepository
                    .GetPagedList(
                        predicate: a =>
                            businessToBusinesses.Contains(a.BusinessId) &&
                            a.EduBusinessType != (int)EduBusinessType.Admin,
                        include: a =>
                            a.Include(b => b.SystemUser).ThenInclude(e => e.EduUser).Include(b => b.SystemUser)
                                .ThenInclude(c => c.Experience), pageSize: int.MaxValue).Items;

            var connections = new List<ConnectionModel>();
            foreach (var cBusiness in businessId2s)
            {
                var connectedSystemUser = cBusiness.SystemUser.FirstOrDefault();
                var connectedEduUser = connectedSystemUser?.EduUser.SingleOrDefault();
                if (connectedSystemUser != null && connectedEduUser != null && !connectedEduUser.IsBlocked && !connectedEduUser.IsHidden)
                    connections.Add(new ConnectionModel()
                    {
                        FirstName = connectedSystemUser.FirstName,
                        LastName = connectedSystemUser.LastName,
                        MiddleName = connectedSystemUser.MiddleName,
                        SystemUserId = connectedSystemUser.SystemUserId,
                        Occupation = MapExperience(connectedSystemUser),
                        MentifiType = MentifiTypeLookup(cBusiness.EduBusinessType),
                        WorkPhone = connectedSystemUser.WorkPhone,
                        MobilePhone = connectedSystemUser.MobilePhone,
                        Email = connectedSystemUser.EmailAddress,
                        ProfilePhoto = connectedSystemUser.ToPhotoUrl(url),
                        Salutation = Salutation(connectedSystemUser.Title),
                        BusinessId = business.BusinessId
                    });
            }
            return connections;
        }

        /// <summary>
        /// un-accepted connection request of current user
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerable<ConnectionModel> GetRequested(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (systemUser == null) throw new ApplicationException("System User is not found ");

            var business = systemUser.Business;
            if (business == null) throw new ApplicationException("Business is not found ");
            if (business.EduBusinessType == 0) throw new ApplicationException("User is not mentifi user");

            var businessId2BusinessToBusinesses = systemUser.Business.BusinessToBusiness
                .Where(a => a.IsPending == true && a.CreatedBy != systemUserId).Select(b => b.BusinessId2);
            var businessTousinesses = systemUser.Business.BusinessToBusiness
                .Where(a => a.IsPending == true && a.CreatedBy != systemUserId).ToList();
            // ReSharper disable once InconsistentNaming
            var businessId2s =
                _businessRepository.GetPagedList(predicate: a => businessId2BusinessToBusinesses.Contains(a.BusinessId),
                    include: a =>
                        a.Include(b => b.SystemUser).ThenInclude(c => c.EduUser).Include(b => b.SystemUser)
                            .ThenInclude(c => c.Experience), pageSize: int.MaxValue).Items;
            var connections = new List<ConnectionModel>();

            foreach (var cBusiness in businessId2s)
            {
                var connectedSystemUser = cBusiness.SystemUser.FirstOrDefault();
                var connectedEduUser = connectedSystemUser?.EduUser.SingleOrDefault();
                if (connectedSystemUser != null && connectedEduUser != null && !connectedEduUser.IsBlocked && !connectedEduUser.IsHidden)
                    connections.Add(new ConnectionModel()
                    {
                        FirstName = connectedSystemUser.FirstName,
                        LastName = connectedSystemUser.LastName,
                        MiddleName = connectedSystemUser.MiddleName,
                        SystemUserId = connectedSystemUser.SystemUserId,
                        Occupation = MapExperience(connectedSystemUser),
                        MentifiType = MentifiTypeLookup(cBusiness.EduBusinessType),
                        WorkPhone = connectedSystemUser.WorkPhone,
                        MobilePhone = connectedSystemUser.MobilePhone,
                        Email = connectedSystemUser.EmailAddress,
                        ProfilePhoto = connectedSystemUser.ToPhotoUrl(url),
                        Salutation = Salutation(connectedSystemUser.Title),
                        BusinessId = business.BusinessId,
                        IsRejected = businessTousinesses.FirstOrDefault(a => a.BusinessId2 == cBusiness.BusinessId)?.IsRejected ?? false

                    });
            }
            return connections;
        }

        /// <summary>
        /// friend request of current user
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerable<ConnectionModel> GetPending(int systemUserId, string url)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));
            if (systemUser == null) throw new ApplicationException("System User is not found ");

            var business = systemUser.Business;
            if (business == null) throw new ApplicationException("Business is not found ");
            if (business.EduBusinessType == 0) throw new ApplicationException("User is not mentifi user");

            var businesToBusinesses =
                _businessToBusinessRepository.GetPagedList(predicate: a =>
                        a.BusinessId2 == business.BusinessId && a.IsPending == true && a.CreatedBy == systemUserId,
                    pageSize: int.MaxValue,
                    include: c =>
                        c.Include(a => a.Business1).ThenInclude(b => b.SystemUser).ThenInclude(d => d.Experience));

            var connections = new List<ConnectionModel>();
            foreach (var cBusiness in businesToBusinesses.Items)
            {
                var connectedSystemUser = cBusiness.Business1.SystemUser.FirstOrDefault();
                if (connectedSystemUser != null)
                    connections.Add(new ConnectionModel
                    {
                        FirstName = connectedSystemUser.FirstName,
                        LastName = connectedSystemUser.LastName,
                        MiddleName = connectedSystemUser.MiddleName,
                        SystemUserId = connectedSystemUser.SystemUserId,
                        ProfilePhoto = connectedSystemUser.ToPhotoUrl(url),
                        Occupation = MapExperience(connectedSystemUser),
                        MentifiType = MentifiTypeLookup(cBusiness.Business1.EduBusinessType),
                        WorkPhone = connectedSystemUser.WorkPhone,
                        MobilePhone = connectedSystemUser.MobilePhone,
                        Email = connectedSystemUser.EmailAddress,
                        Salutation = Salutation(connectedSystemUser.Title),
                        BusinessId = business.BusinessId,
                        IsRejected = cBusiness.IsRejected
                    });
            }
            return connections;
        }

        private string EduBusinessTypeName(int eduBusinessType)
        {
            switch (eduBusinessType)
            {
                case (int)EduBusinessType.Mentor:
                    return "Mentor";
                case (int)EduBusinessType.Mentee:
                    return "Mentee";
                case (int)EduBusinessType.Admin:
                    return "Admin";
                default: return string.Empty;
            }
        }

        public async Task Request(int senderSystemUserId, int receiverSystemUserId, string message)
        {
            var senderSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == senderSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var receiverSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == receiverSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (senderSystemUser == null)
                throw new ApplicationException("The Sender System User Id is invalid.");
            if (receiverSystemUser == null)
                throw new ApplicationException("The Receiver System User Id is invalid.");
            if (senderSystemUser.Business.EduBusinessType == 0)
                throw new ApplicationException("The Sender System User  is not mentifi user.");
            if (receiverSystemUser.Business.EduBusinessType == 0)
                throw new ApplicationException("The Receiver System User  is not mentifi user.");

            if (senderSystemUser.Business.EduBusinessType == receiverSystemUser.Business.EduBusinessType)
                throw new ApplicationException(
                    $"{EduBusinessTypeName(senderSystemUser.Business.EduBusinessType)} should not be able to send connect request to another {EduBusinessTypeName(senderSystemUser.Business.EduBusinessType)}");


            var universitySetting = _eduUniversityRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId == (senderSystemUser.Business.UniversityId ?? senderSystemUser.BusinessId));

            var isValidated = ValidateRequestingLimit(senderSystemUser.Business, universitySetting) &&
                              ValidateAcceptingtLimit(senderSystemUser.Business, universitySetting, false) &&
                              ValidateAcceptingtLimit(receiverSystemUser.Business, universitySetting, true);

            if (isValidated)
            {
                var oldBusinessToBusiness1 =
                    senderSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                        a.BusinessId2 == receiverSystemUser.BusinessId);

                var oldBusinessToBusiness2 =
                    receiverSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                        a.BusinessId2 == senderSystemUser.BusinessId);
                if (oldBusinessToBusiness1 != null)
                    _businessToBusinessRepository.Delete(oldBusinessToBusiness1);
                if (oldBusinessToBusiness2 != null)
                    _businessToBusinessRepository.Delete(oldBusinessToBusiness2);

                var businessToBusiness1 = new BusinessToBusiness
                {
                    BusinessLinkId = 0,
                    BusinessId1 = senderSystemUser.BusinessId,
                    BusinessId2 = receiverSystemUser.BusinessId,
                    IsPending = true,
                    IsActive = false,
                    IsRejected = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = senderSystemUserId,
                    IsClient = null,
                    IsNetwork = null,
                    IsSupplier = null,
                    ModifiedBy = null,
                    ModifiedOn = null,
                    IsSubsidiary = null
                };
                _businessToBusinessRepository.Insert(businessToBusiness1);

                var businessToBusiness2 = new BusinessToBusiness
                {
                    BusinessId1 = receiverSystemUser.BusinessId,
                    BusinessId2 = senderSystemUser.BusinessId,
                    IsPending = true,
                    IsActive = false,
                    IsRejected = false,
                    CreatedOn = DateTime.Now,
                    CreatedBy = senderSystemUserId,
                    IsClient = null,
                    IsNetwork = null,
                    IsSupplier = null,
                    IsSubsidiary = null,
                    ModifiedBy = null,
                    ModifiedOn = null
                };
                _businessToBusinessRepository.Insert(businessToBusiness2);

                _mentifiApplicationUsageRepository.Insert(new MentifiApplicationUsage()
                {
                    BusinessId = universitySetting.BusinessId,
                    CreatedOn = DateTime.Now,
                    SystemUserId = senderSystemUserId,
                    MentifiApplicationId = (int)EduApplication.InvitationSent
                });

                _mentifiApplicationUsageRepository.Insert(new MentifiApplicationUsage()
                {
                    BusinessId = universitySetting.BusinessId,
                    CreatedOn = DateTime.Now,
                    SystemUserId = receiverSystemUserId,
                    MentifiApplicationId = (int)EduApplication.InvitationReceived
                });

                CreateNotif(receiverSystemUserId, senderSystemUserId, Constant.NOTIFTYPE_BUSINESSLINK,
                    $"{senderSystemUser.FullName} has sent you a connection request.");

                AddMessageBoard(senderSystemUser, receiverSystemUser, message);
                _unitOfWork.SaveChanges();

                await _emailApi.RequestConnection(new EmailParam()
                {
                    Recipient = new[]
                    {
                        receiverSystemUser.EmailAddress,
                    },
                    SystemUserId = senderSystemUserId,
                    Payload = new[]
                    {
                        receiverSystemUser.FullName,
                        senderSystemUser.FullName,
                        universitySetting.UniversityNameAlias,
                        MentifiTypeLookup(receiverSystemUser.Business.EduBusinessType).Name,
                        universitySetting.ProgramName,
                        message,
                        _configuration["MentifiWebUrl"],
                        MentifiTypeLookup(senderSystemUser.Business.EduBusinessType).Name
                    }
                });
            }
        }

        private void AddMessageBoard(SystemUser sender, SystemUser receiver, string message)
        {
            _messageBoardService.RequestConnection(new RequestedConnectionMessageBoardModel()
            {
                FromSystemUserId = sender.SystemUserId,
                FromBusinessId = sender.BusinessId,
                Body = message,
                ToBusinessId = receiver.BusinessId,
                ToSystemUserId = receiver.SystemUserId
            });
        }

        public async Task Accept(int senderSystemUserId, int receiverSystemUserId)
        {
            var senderSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == senderSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var receiverSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == receiverSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (senderSystemUser == null)
                throw new ApplicationException("Sender System User Id is invalid.");

            if (receiverSystemUser == null)
                throw new ApplicationException("Receiver System User Id is invalid.");

            var universitySetting = _eduUniversityRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId == (senderSystemUser.Business.UniversityId ?? senderSystemUser.BusinessId));

            var isValid = ValidateAcceptingtLimit(senderSystemUser.Business, universitySetting, false);
            if (isValid)
            {

                var businessToBusiness1 =
                    senderSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                        a.BusinessId2 == receiverSystemUser.BusinessId);
                var businessToBusiness2 =
                    receiverSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                        a.BusinessId2 == senderSystemUser.BusinessId);

                if (businessToBusiness1 != null)
                {
                    businessToBusiness1.IsPending = false;
                    businessToBusiness1.IsRejected = false;
                    businessToBusiness1.IsActive = true;
                    businessToBusiness1.ModifiedOn = DateTime.Now;
                    businessToBusiness1.ModifiedBy = receiverSystemUserId;
                    _businessToBusinessRepository.Update(businessToBusiness1);
                }

                if (businessToBusiness2 != null)
                {
                    businessToBusiness2.IsPending = false;
                    businessToBusiness2.IsRejected = false;
                    businessToBusiness2.IsActive = true;
                    businessToBusiness2.ModifiedOn = DateTime.Now;
                    businessToBusiness2.ModifiedBy = receiverSystemUserId;
                    _businessToBusinessRepository.Update(businessToBusiness2);
                }

                CreateNotif(receiverSystemUserId, senderSystemUserId, Constant.NOTIFTYPE_LINKACTIVATE,
                    $"{senderSystemUser.FullName} has connected with you.");

                _unitOfWork.SaveChanges();

                await _emailApi.AcceptConnection(new EmailParam()
                {
                    Recipient = new[]
                    {
                        receiverSystemUser.EmailAddress,
                    },
                    SystemUserId = senderSystemUserId,
                    Payload = new[]
                    {
                        receiverSystemUser.FullName,
                        senderSystemUser.FullName,
                        universitySetting.UniversityNameAlias,
                        MentifiTypeLookup(receiverSystemUser.Business.EduBusinessType).Name,
                        universitySetting.ProgramName,
                        "",
                        _configuration["MentifiWebUrl"],
                        ""
                    }

                });
            }
        }

        public async Task Reject(int senderSystemUserId, int receiverSystemUserId, string message)
        {
            var senderSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == senderSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var receiverSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == receiverSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (senderSystemUser == null)
            {
                throw new ApplicationException("The Sender System User Id is invalid.");
            }
            if (receiverSystemUser == null)
            {
                throw new ApplicationException("The Receiver System User Id is invalid.");
            }
            var businessToBusiness1 =
                senderSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == receiverSystemUser.BusinessId);
            var businessToBusiness2 =
                receiverSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == senderSystemUser.BusinessId);


            if (businessToBusiness1 != null)
            {
                businessToBusiness1.IsPending = true;
                businessToBusiness1.IsRejected = true;
                businessToBusiness1.IsActive = false;
                businessToBusiness1.ModifiedOn = DateTime.Now;
                businessToBusiness1.ModifiedBy = receiverSystemUserId;
                _businessToBusinessRepository.Update(businessToBusiness1);

            }

            if (businessToBusiness2 != null)
            {
                businessToBusiness2.IsPending = true;
                businessToBusiness2.IsRejected = true;
                businessToBusiness2.IsActive = false;
                businessToBusiness2.ModifiedOn = DateTime.Now;
                businessToBusiness2.ModifiedBy = receiverSystemUserId;
                _businessToBusinessRepository.Update(businessToBusiness2);
            }

            CreateNotif(receiverSystemUserId, senderSystemUserId, Constant.NOTIFTYPE_EDUREJECT,
                $"{senderSystemUser.FullName} has rejected your connection request.");
            _unitOfWork.SaveChanges();

            await _emailApi.RejectConnection(new EmailParam
            {
                Recipient = new[]
                {
                    receiverSystemUser.EmailAddress
                },
                SystemUserId = senderSystemUserId,
                Payload = new[]
                {
                    senderSystemUser.FullName,
                    message
                }
            });

        }

        public void Cancel(int senderSystemUserId, int receiverSystemUserId)
        {
            var senderSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == senderSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var receiverSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == receiverSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (senderSystemUser == null)
            {
                throw new ApplicationException("The Sender System User Id is invalid.");
            }
            if (receiverSystemUser == null)
            {
                throw new ApplicationException("The Receiver System User Id is invalid.");
            }
            var businessToBusiness1 =
                senderSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == receiverSystemUser.BusinessId);
            var businessToBusiness2 =
                receiverSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == senderSystemUser.BusinessId);
            _businessToBusinessRepository.Delete(businessToBusiness1, businessToBusiness2);
            _unitOfWork.SaveChanges();
        }

        public void Remove(int senderSystemUserId, int receiverSystemUserId)
        {
            var senderSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == senderSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var receiverSystemUser = _systemUserRepository.GetFirstOrDefault(
                predicate: a => a.SystemUserId == receiverSystemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            if (senderSystemUser == null)
            {
                throw new ApplicationException("The Sender System User Id is invalid.");
            }
            if (receiverSystemUser == null)
            {
                throw new ApplicationException("The Receiver System User Id is invalid.");
            }
            var businessToBusiness1 =
                senderSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == receiverSystemUser.BusinessId);
            var businessToBusiness2 =
                receiverSystemUser.Business.BusinessToBusiness.FirstOrDefault(a =>
                    a.BusinessId2 == senderSystemUser.BusinessId);
            _businessToBusinessRepository.Delete(businessToBusiness1, businessToBusiness2);
            if (businessToBusiness1?.CreatedBy == senderSystemUserId)
            {
                CreateNotif(receiverSystemUserId, senderSystemUserId,
                    Constant.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_FROM,
                     receiverSystemUser.BusinessId.ToString());
                CreateNotif(senderSystemUserId, receiverSystemUserId,
                    Constant.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_TO,
                    receiverSystemUser.BusinessId.ToString());
            }
            else
            {
                CreateNotif(senderSystemUserId, receiverSystemUserId,
                      Constant.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_FROM,
                      receiverSystemUser.BusinessId.ToString());
                CreateNotif(receiverSystemUserId, senderSystemUserId,
                    Constant.REMOVE_CONNECTION_BY_MENTOR_OR_MENTEE_TO,
                    receiverSystemUser.BusinessId.ToString());
            }
            _unitOfWork.SaveChanges();
        }

        public ConnectionCountModel GetCounts(int systemUserId)
        {
            var result = _systemUserRepository.GetFirstOrDefault(a => new { a.Business.BusinessToBusiness, a.Business },
                predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));

            var pending = result.BusinessToBusiness.Count(a => a.IsPending == true && a.CreatedBy == systemUserId);
            var active = result.BusinessToBusiness.Where(a => a.IsActive == true).Select(a => a.BusinessId2).ToArray();
            var connectedCount = _businessRepository.Count(a =>
                active.Contains(a.BusinessId) && a.EduBusinessType != (int)EduBusinessType.Admin);
            var requested = _businessToBusinessRepository.Count(a =>
                a.IsPending == true && a.BusinessId2 == result.Business.BusinessId && a.CreatedBy != systemUserId);

            return new ConnectionCountModel
            {
                Requested = requested,
                Connected = connectedCount,
                Pending = pending
            };
        }

        public AdminConnectionCountModel GetAdminCounts(int systemUserId)
        {
            var businessToBusinesses = _systemUserRepository.GetFirstOrDefault(
                selector: a => a.Business.BusinessToBusiness.Select(b => b.BusinessId2),
                predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business).ThenInclude(c => c.BusinessToBusiness));
            var busineses =
                _businessRepository.GetPagedList(selector: a => new { a.EduBusinessType },
                    predicate: a => businessToBusinesses.Contains(a.BusinessId), pageSize: int.MaxValue).Items;
            return new AdminConnectionCountModel()
            {
                Mentor = busineses.Count(a => a.EduBusinessType == (int)EduBusinessType.Mentor),
                Mentee = busineses.Count(a => a.EduBusinessType == (int)EduBusinessType.Mentee)
            };
        }

        public LookupModel<int> GetStatus(int fromSystemUserId, int toSystemUserId)
        {
            var from = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == fromSystemUserId);
            var to = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == toSystemUserId);
            var businessToBusiness = _businessToBusinessRepository.GetFirstOrDefault(predicate: a =>
                a.BusinessId1 == from.BusinessId && a.BusinessId2 == to.BusinessId);

            if (businessToBusiness == null)
                return new LookupModel<int>("Removed", (int)ConnectionStatus.Removed);

            if (businessToBusiness.IsRejected)
                return new LookupModel<int>("Rejected", (int)ConnectionStatus.Rejected);

            if (businessToBusiness.IsPending == true && businessToBusiness.CreatedBy != fromSystemUserId)
                return new LookupModel<int>("Pending", (int)ConnectionStatus.Pending);

            if (businessToBusiness.IsPending == true && businessToBusiness.CreatedBy == fromSystemUserId)
                return new LookupModel<int>("Request", (int)ConnectionStatus.Request);

            if (businessToBusiness.IsActive == true)
                return new LookupModel<int>("Connected", (int)ConnectionStatus.Approved);

            throw new ApplicationException("No Connection");
        }

        private bool ValidateAcceptingtLimit(Business accepterBusiness, EduUniversity universitySetting, bool other)
        {
            var connectedCount =
                _businessToBusinessRepository.Count(predicate: a =>
                    a.BusinessId1 == accepterBusiness.BusinessId && a.IsPending == false && a.IsActive == true && !a.IsRejected);
            var label = other
                ? (accepterBusiness.EduBusinessType == (int)EduBusinessType.Mentor
                    ? universitySetting.MentorAlias
                    : universitySetting.MenteeAlias) + " already"
                : "You";
            if (accepterBusiness.EduBusinessType == (int)EduBusinessType.Mentee)
            {
                if (universitySetting.MaxNumberMentorForMentee < connectedCount)
                {
                    throw new ApplicationException(
                        $"{label} have {universitySetting.MaxNumberMentorForMentee} {universitySetting.MenteeAlias.ToLower()}(s)");
                }
            }
            else if (accepterBusiness.EduBusinessType == (int)EduBusinessType.Mentor)
            {
                if (universitySetting.MaxNumberMenteeForMentor < connectedCount)
                {
                    throw new ApplicationException(
                        $"{label} have {universitySetting.MaxNumberMenteeForMentor} {universitySetting.MentorAlias.ToLower()}(s)");
                }
            }
            return true;
        }

        private bool ValidateRequestingLimit(Business connecterBusiness, EduUniversity universitySetting)
        {
            var connections = connecterBusiness.BusinessToBusiness;

            if (connecterBusiness.EduBusinessType == (int)EduBusinessType.Mentee)
            {
                var pendingCount = connections.Count(a => a.IsPending == true);
                if (pendingCount >= universitySetting.MaxMenteeRequest)
                {
                    throw new ApplicationException($"You already have {pendingCount} pending connect requests");
                }
            }

            return true;
        }

        private LookupModel<int> MentifiTypeLookup(int eduBusinessType)
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
                    throw new ApplicationException("User is not mentifi user");
            }
        }

        private ExperienceModel MapExperience(SystemUser systemUser)
        {

            var experiences = systemUser.Experience;
            var currentJob = experiences.FirstOrDefault(b => b.IsCurrentlyWorkHere == true);
            if (currentJob != null)
            {
                return new ExperienceModel()
                {
                    SystemUserId = currentJob.SystemUserId,
                    ExperienceId = currentJob.ExperienceId,
                    TimePeriodStart = currentJob.TimePeriodStart.ToUnixTime(),
                    TimePeriodEnd = currentJob.TimePeriodEnd.ToUnixTime(),
                    CompanyName = currentJob.CompanyName,
                    Description = currentJob.Description,
                    EduExperienceInMonths = currentJob.EduExperienceInMonths,
                    EduExperienceInYears = currentJob.EduExperienceInYears,
                    Location = currentJob.Location,
                    Title = currentJob.Title
                };
            }

            return null;
        }

        private void CreateNotif(int systemUserId, int createdBy, string notifType, string message)
        {
            var model = new Notification
            {
                SystemUserId = systemUserId,
                CreatedOn = DateTime.Now,
                CreatedBy = createdBy,
                SystemUserType = Constant.USERTYPE_ADVISER,
                NotificationType = notifType,
                IsShowed = false,
                Message = message
            };
            _notificationRepository.Insert(model);

            _busInstance.Publish(new NotificationAdded
            {
                NotificationType = notifType,
                SystemUserID = systemUserId,
                SystemUserType = Constant.USERTYPE_ADVISER,
                CreatedOn = DateTime.UtcNow,
                IsShowed = false,
                CreatedBy = createdBy,
                Message = message,
            });
        }

        private LookupModel<string> Salutation(string title)
        {
            if (string.IsNullOrEmpty(title))
                return null;

            var id = int.Parse(title);
            var lookups = _lookupService.GetByAttributeName(Constant.LookupTypeCode_Salutation);
            return new LookupModel<string>(
                lookups.FirstOrDefault(b => b.Id == id)?.Name ?? string.Empty,
                title);
        }

        public IEnumerable<AdminModel> GetAdmin(int systemUserId, string url)
        {
            var admin = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId, include: a => a.Include(b => b.Business));
            if (admin == null)
                throw new ApplicationException("University admin is invalid");
            if (admin.Business.EduBusinessType != (int)EduBusinessType.Admin)
                throw new ApplicationException("User is not administrator");

            var university = _eduUniversityRepository.GetFirstOrDefault(predicate: a => a.BusinessId == (admin.Business.UniversityId ?? admin.BusinessId));
            if (university == null)
                throw new ApplicationException("system user id is not mentifi user.");

            return _businessToBusinessRepository.GetPagedList(selector: a => Map(a, url), predicate: a => a.BusinessId2 == university.BusinessId && a.IsActive == true, include: a => a.Include(b => b.Business1).ThenInclude(c => c.SystemUser), pageSize: int.MaxValue).Items;
        }

        private AdminModel Map(BusinessToBusiness a, string url)
        {
            var systemUser = a.Business1.SystemUser.SingleOrDefault();
            return new AdminModel
            {
                Email = systemUser.EmailAddress,
                FullName = systemUser.FullName,
                JoinDate = a.CreatedOn.ToUnixTime(),
                PhotoUrl = systemUser.ToPhotoUrl(url)
            };
        }
    }

    public class AdminModel
    {
        public string PhotoUrl { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public long? JoinDate { get; set; }
    }

}
