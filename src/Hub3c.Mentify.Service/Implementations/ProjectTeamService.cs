using System.Collections.Generic;
using Hub3c.Mentify.Repository.Models;
using Hub3c.Mentify.Service.Helpers;
using Hub3c.Mentify.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace Hub3c.Mentify.Service.Implementations
{
    public class ProjectTeamService : IProjectTeamService
    {
        private readonly IRepository<ProjectTeam> _projectTeamRepository;
        public ProjectTeamService(IUnitOfWork unitOfWork)
        {
            _projectTeamRepository = unitOfWork.GetRepository<ProjectTeam>();
        }

        public IEnumerable<ProfileModelIncludingEmail> GetByProject(int projectId, string baseUrl)
        {
           return _projectTeamRepository.GetPagedList(selector: a => a.User.ToProfileModelIncludingEmail(baseUrl), predicate: a => a.ProjectId == projectId).Items;
        }
    }
}
