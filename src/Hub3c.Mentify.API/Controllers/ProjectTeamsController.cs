using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Authorize]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Projects/{projectId}/ProjectTeams")]
    [ApiVersion("1")]
    public class ProjectTeamsController : Controller
    {
        private readonly IProjectTeamService _projectTeamService;

        /// <inheritdoc />
        public ProjectTeamsController(IProjectTeamService projectTeamService)
        {
            _projectTeamService = projectTeamService;
        }

        /// <summary>
        /// Get Project Teams
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProfileModel), 200)]
        public IActionResult Get([FromRoute] int projectId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_projectTeamService.GetByProject(projectId, baseUrl)));
        }
    }
}