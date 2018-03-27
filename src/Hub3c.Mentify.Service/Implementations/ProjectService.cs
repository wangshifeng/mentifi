using System.Collections.Generic;
using System.Linq;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class ProjectService : IProjectService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<SystemUser> _systemUserRepository;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _projectRepository = unitOfWork.GetRepository<Project>();
            _systemUserRepository = unitOfWork.GetRepository<SystemUser>();
        }

        public IEnumerable<ProjectModel> GetBySystemUserId(int systemUserId, string baseUrl)
        {
            var systemUser = _systemUserRepository.GetFirstOrDefault(predicate: a => a.SystemUserId == systemUserId,
                include: a => a.Include(b => b.Business));

            return _projectRepository.GetPagedList(selector: a => Mapping(a, baseUrl),
             predicate: a => a.CreatedBy == systemUser.SystemUserId && a.IsDeleted == false,
             include: a => a.Include(b => b.CreatedBySystemUser), pageSize: int.MaxValue).Items;
        }

        public IEnumerable<ProjectModel> GetByUniversityId(SystemUser systemUser, string baseUrl)
        {

            var members = _systemUserRepository.GetPagedList(predicate: a => a.Business.UniversityId == systemUser.BusinessId,
                include: a => a.Include(b => b.Business), pageSize: int.MaxValue).Items.Select(a => a.SystemUserId).ToArray();

            return _projectRepository.GetPagedList(selector: a => Mapping(a, baseUrl),
                predicate: a => a.CreatedBy != null && members.Contains(a.CreatedBy.Value) && a.IsDeleted == false,
                include: a => a.Include(b => b.CreatedBySystemUser), pageSize: int.MaxValue).Items;
        }

        private ProjectModel Mapping(Project project, string baseUrl)
        {
            return new ProjectModel
            {
                Id = project.ProjectId,
                CreatedBy = project.CreatedBySystemUser.ToProfileModel(baseUrl),
                Description = project.Description,
                ProjectName = project.ProjectName,
            };
        }


    }
}
