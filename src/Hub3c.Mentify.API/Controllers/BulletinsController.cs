using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Bulletins")]
    [ApiVersion("1")]
    [Authorize]
    public class 
        BulletinsController : Controller
    {
        private readonly IBulletinService _bulletinService;

        /// <inheritdoc />
        public BulletinsController(IBulletinService bulletinService)
        {
            _bulletinService = bulletinService;
        }

        /// <summary>
        /// Get Personal Bulletin
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{systemUserId}/Private")]
        public IActionResult Private([FromRoute] int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_bulletinService.GetPrivate(systemUserId, baseUrl)));
        }

        /// <summary>
        /// Get Network Bulletin 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{systemUserId}/Network")]
        public IActionResult Network([FromRoute] int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_bulletinService.GetPublic(systemUserId, baseUrl)));
        }

        /// <summary>
        /// Get Comments of  Bulletin 
        /// </summary>
        /// <param name="bulletinId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{bulletinId}/Comments")]
        public IActionResult Comments([FromRoute] int bulletinId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_bulletinService.GetComments(bulletinId, baseUrl)));
        }
    }
}