using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Networks")]
    [Authorize]
    [ApiVersion("1")]
    [ApiVersion("2")]
    public class NetworksController : Controller
    {
        private readonly IConnectionService _connectionService;

        /// <inheritdoc />
        public NetworksController(IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Get list of user's request mentor or mentee
        /// </summary>
        /// <param name="systemUserId">System User ID</param>
        /// <returns></returns>
        [Route("{systemUserId}/Requested")]
        [HttpGet]
        public IActionResult Requested([FromRoute]int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

            var connecteds = _connectionService.GetRequested(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(connecteds));
        }

        /// <summary>
        /// Get list of user's connected mentor or mentee
        /// </summary>
        /// <param name="systemUserId">System User  Id</param>
        /// <returns></returns>
        [Route("{systemUserId}/Connected")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Connected([FromRoute]int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var connecteds = _connectionService.GetConnected(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(connecteds));
        }

        /// <summary>
        /// Get list of user's pending mentor or mentee
        /// </summary>
        /// <param name="systemUserId">System User ID</param>
        /// <returns></returns>
        [Route("{systemUserId}/Pending")]
        [HttpGet]
        public IActionResult Pending([FromRoute]int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var connecteds = _connectionService.GetPending(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(connecteds));
        }

        /// <summary>
        /// Get Admin of university
        /// </summary>
        /// <param name="systemUserId">System User Id</param>
        /// <returns></returns>
        [Route("{systemUserId}/Admin")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Admin([FromRoute]int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var connecteds = _connectionService.GetAdmin(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(connecteds));
        }
    }
}