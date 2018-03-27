using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Mentees")]
    [ApiVersion("1")]
    public class MenteesController : Controller
    {
        private readonly IConnectionService _connectionService;

        /// <inheritdoc />
        public MenteesController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Get All Mentee
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{systemUserId}")]
        public IActionResult Get([FromRoute] int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

            var mentees = _connectionService.GetMentee(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(mentees));
        }
    }
}