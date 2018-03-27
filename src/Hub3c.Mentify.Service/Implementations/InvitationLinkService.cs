using Hub3c.Mentify.Repository.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hub3c.Mentify.Service.Implementations
{
    public class InvitationLinkService : IInvitationLinkService
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<SystemUser> _systemUserRepository;
        private IRepository<InvitationLink> _invitationLinkRepository;
        public InvitationLinkService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _invitationLinkRepository = unitOfWork.GetRepository<InvitationLink>();
        }

        public string AdminInvitationLink(int systemUserId)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId);
            var randomUrl = RandomString(5);
            var model = new InvitationLink()
            {
                CreatedBy = systemUser.SystemUserId,
                BusinessID = systemUser.BusinessId,
                SystemUserID = systemUser.SystemUserId,
                CreatedOn = DateTime.UtcNow,
                InvitationType = 4,
                UniqueUrl = randomUrl
            };
            _invitationLinkRepository.Insert(model);
            _unitOfWork.SaveChanges();
            return randomUrl;
        }

        private string RandomString(int length)
        {
            var random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
