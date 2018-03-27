using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Projects")]
    [ApiVersion("1")]
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;

        /// <inheritdoc />
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Get University Projects or Personal Project
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ProjectModel), 200)]
        public IActionResult Get([FromQuery] int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_projectService.GetBySystemUserId(systemUserId, baseUrl)));
        }
    }
}