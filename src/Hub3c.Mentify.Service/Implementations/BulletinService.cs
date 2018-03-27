using System.Collections.Generic;
using System.Linq;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;
using BusinessToBusiness = Hub3c.Mentify.Repository.Models.BusinessToBusiness;
using SystemUser = Hub3c.Mentify.Repository.Models.SystemUser;

namespace Hub3c.Mentify.Service.Implementations
{
    public class BulletinService : IBulletinService
    {
        private readonly IRepository<BusinessBulletinBoard> _bulletinRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        private readonly IRepository<BusinessToBusiness> _businessToBusinessRepository;
        private readonly IRepository<DocumentRegister> _documentRegisteRepository;

        public BulletinService(IUnitOfWork unitOfWork)
        {
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
            _bulletinRepository = unitOfWork.GetRepository<BusinessBulletinBoard>();
            _businessToBusinessRepository = unitOfWork.GetRepository<BusinessToBusiness>();
            _documentRegisteRepository = unitOfWork.GetRepository<DocumentRegister>();
        }

        public IEnumerable<BulletinModel> GetPrivate(int systemUserId, string url)
        {
            var myBulletins = _bulletinRepository.GetPagedList(selector: a => Mapping(a), predicate: a =>
                 a.CreatedBy == systemUserId && a.BusinessPostId == a.RegardingPostId && a.IsPrivatePost == true, pageSize:int.MaxValue);

            var friendsBulletin = _bulletinRepository.GetPagedList(selector: a => Mapping(a), predicate: a =>
                 a.RecipientList.Contains($";{systemUserId};") && a.BusinessPostId == a.RegardingPostId && a.IsPrivatePost == true);
            var models = myBulletins.Items.Union(friendsBulletin.Items).ToList();
            var systemUsers = myBulletins.Items.Select(a => a.SystemUserId)
                .Union(friendsBulletin.Items.Select(a => a.SystemUserId)).ToArray();
            var systemUserModels = _systemUserRepository.GetPagedList(predicate: a => systemUsers.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            var bulletinIds = models.Select(a => a.Id).ToArray();
            var attachments = _documentRegisteRepository.GetPagedList(predicate: a =>
                a.RegardingEntityName == Constant.STATIC_BULLETINATTACHMENT &&
                bulletinIds.Contains(a.RegardingEntityId.Value), pageSize: int.MaxValue).Items;
            foreach (var model in models)
            {
                var systemUserModel = systemUserModels.FirstOrDefault(a => a.SystemUserId == model.SystemUserId);
                model.FullName = systemUserModel?.FullName;
                model.SystemUserId = systemUserModel?.SystemUserId;
                model.ProfilePhotoUrl = systemUserModel.ToPhotoUrl(url);
                model.CommentCount = _bulletinRepository.Count(a => a.RegardingPostId == model.Id);
                model.Attachments = MappingAttachment(model.Id, attachments, url);
            }
            return models;
        }

        public IEnumerable<BulletinModel> GetPublic(int systemUserId, string url)
        {
            var myBulletins = _bulletinRepository.GetPagedList(selector: a => Mapping(a), predicate: a =>
                a.SystemUserId == systemUserId && a.BusinessPostId == a.RegardingPostId && a.IsBusinessOnly == false &&
                a.IsPrivatePost == false, pageSize: int.MaxValue);

            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business));

            var networks = _businessToBusinessRepository.GetPagedList(selector: a => a.Business1.SystemUser,
                    predicate: a =>
                        a.BusinessId2 == (systemUser.Business.UniversityId ?? systemUser.BusinessId) &&
                        a.IsActive == true && a.BusinessId2 != null && a.Business1.EduBusinessType != 0,
                    include: a => a.Include(b => b.Business1).ThenInclude(c => c.SystemUser), pageSize: int.MaxValue)
                .Items
                .Distinct();

            var networkSystemUserIds = networks.SelectMany(a => a).Select(a => a.SystemUserId).ToArray();
            var friendsBulletin = _bulletinRepository.GetPagedList(selector: a => Mapping(a),
                predicate: a =>
                    a.SystemUserId != null && networkSystemUserIds.Contains(a.CreatedBy.Value) &&
                    a.BusinessPostId == a.RegardingPostId && a.IsBusinessOnly == false && a.IsPrivatePost == false,
                pageSize: int.MaxValue);

            var models = myBulletins.Items.Union(friendsBulletin.Items).ToList();
            var systemUsers = myBulletins.Items.Select(a => a.SystemUserId)
                .Union(friendsBulletin.Items.Select(a => a.SystemUserId)).ToArray();

            var systemUserModels = _systemUserRepository
                .GetPagedList(selector: a => new {a.SystemUserId, a.FullName, a.ProfilePhoto},
                    predicate: a => systemUsers.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;

            var bulletinIds = models.Select(a => a.Id).ToArray();
            var attachments = _documentRegisteRepository.GetPagedList(predicate: a =>
                      a.RegardingEntityName == Constant.STATIC_BULLETINATTACHMENT &&
                      bulletinIds.Contains(a.RegardingEntityId.Value), pageSize: int.MaxValue).Items;

            foreach (var model in models)
            {
                var systemUserModel = systemUserModels.FirstOrDefault(a => a.SystemUserId == model.SystemUserId);
                model.FullName = systemUserModel?.FullName;
                model.SystemUserId = systemUserModel?.SystemUserId;
                model.ProfilePhotoUrl = systemUser.ToPhotoUrl(url);
                model.CommentCount = _bulletinRepository.Count(a => a.RegardingPostId == model.Id);
                model.Attachments = MappingAttachment(model.Id, attachments, url);
            }
            return models;
        }

        public IEnumerable<CommentModel> GetComments(int bulletinId, string url)
        {
            var comments = _bulletinRepository
                .GetPagedList(predicate: a => a.RegardingPostId == bulletinId && a.BusinessPostId != bulletinId,
                    pageSize: int.MaxValue).Items;

            var systemUserIds = comments.Select(a => a.SystemUserId).ToArray();
            var systemUsers = _systemUserRepository
                .GetPagedList(predicate: a => systemUserIds.Contains(a.SystemUserId), pageSize: int.MaxValue).Items;
            return (from comment in comments
                    let systemUser = systemUsers.FirstOrDefault(a => a.SystemUserId == comment.SystemUserId)
                    select MappingComment(comment, url, systemUser)).ToList();
        }

        private CommentModel MappingComment(BusinessBulletinBoard comment, string url, SystemUser systemUser)
        {
            return new CommentModel
            {
                CreatedOn = comment.CreatedOn.ToUnixTime(),
                SystemUserId = comment.SystemUserId,
                Content = comment.Message,
                ProfilePhotoUrl = systemUser.ToPhotoUrl(url),
                FullName = systemUser?.FullName,
                Id = comment.BusinessPostId,
                Email = systemUser?.EmailAddress
            };
        }

        private IEnumerable<AttachmentModel> MappingAttachment(int bulletinId, IEnumerable<DocumentRegister> attachments, string url)
        {
            return attachments.Where(a => a.RegardingEntityId == bulletinId).Select(a => new AttachmentModel
            {
                Name = a.DocumentName,
                Id = a.DocumentId,
                Url = url + $"/Documents/{a.DocumentId}"
            });
        }

        private BulletinModel Mapping(BusinessBulletinBoard a)
        {
            return new BulletinModel
            {
                Id = a.BusinessPostId,
                SystemUserId = a.SystemUserId ?? 0,
                LikeCount = a.Likes ?? 0,
                DislikeCount = a.Dislikes ?? 0,
                CreatedOn = a.CreatedOn.ToUnixTime(),
                ModifiedOn = a.ModifiedOn.ToUnixTime(),
                EnableLike = !a.LikesList.Contains($";{a.SystemUserId};"),
                EnableDisLike = !a.LikesList.Contains($";{a.SystemUserId};"),
                Message = a.Message,
                Subject = a.Subject
            };
        }
    }
}
