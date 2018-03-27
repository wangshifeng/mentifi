using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IProjectTeamService
    {
        IEnumerable<ProfileModelIncludingEmail> GetByProject(int projectId, string baseUrl);
    }
}
