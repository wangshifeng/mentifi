using System.Collections.Generic;
using Hub3c.Mentify.Service.Models;

namespace Hub3c.Mentify.Service
{
    public interface IProjectService
    {
        IEnumerable<ProjectModel> GetBySystemUserId(int systemUserId, string baseUrl);
    }
}
