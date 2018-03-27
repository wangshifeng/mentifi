using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Hub3c.Mentify.Service.Implementations
{
    public class AdminInvitationService : IAdminInvitationService
    {
        private readonly IEmailApi _emailApi;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<Business> _businessRepository;
        private readonly IConfiguration _configuration;
        private readonly IInvitationLinkService _invitationLinkService;
        public AdminInvitationService(
            IEmailApi emailApi,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IInvitationLinkService invitationLinkService)
        {
            _emailApi = emailApi;
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _businessRepository = unitOfWork.GetRepository<Business>();
            _configuration = configuration;
            _invitationLinkService = invitationLinkService;
        }


        public void Send(AdminInvitationModel model)
        {
            var adminUniversity = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == model.AdminSystemUserId, include: a => a.Include(b => b.Business));
            if (adminUniversity == null)
                throw new ApplicationException("Admin id is invalid");
            var university = adminUniversity.Business;
            if (adminUniversity.Business.UniversityId.HasValue)
            {
                university = _businessRepository.GetFirstOrDefault(predicate: a => a.BusinessId == adminUniversity.Business.UniversityId);
            }
            var randomParam = _invitationLinkService.AdminInvitationLink(model.AdminSystemUserId);
            foreach (var admin in model.AdminInvitationUserModels)
            {
                _emailApi.InviteAdmin(new EmailParam
                {
                    Recipient = new[]
                    {
                        admin.Email,
                        "alvian.hutahaean@geekseat.com.au"
                    },
                    SystemUserId = adminUniversity.SystemUserId,
                    Payload = new string[]
                    {
                        admin.Fullname,
                        adminUniversity.FullName,
                        university.BusinessName,
                        $"{model.RegisterUrl}{randomParam}"
                    }
                });
            }

        }
    }
}
