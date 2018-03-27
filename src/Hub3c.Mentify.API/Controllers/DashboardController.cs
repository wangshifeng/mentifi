using System.Threading.Tasks;
using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}")]
    [Authorize]
    [ApiVersion("1")]
    public class DashboardController : Controller
    {
        private readonly IConnectionService _connectionService;
        private readonly IDashboardService _dashboardService;

        /// <inheritdoc />
        public DashboardController(IConnectionService connectionService, IDashboardService dashboardService)
        {
            _connectionService = connectionService;
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get count of mentee or mentor of user
        /// </summary>
        /// <param name="systemUserId">system user Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Dashboards")]
        public IActionResult Get([FromRoute]int systemUserId)
        {
            var count = _connectionService.GetCounts(systemUserId);
            return Ok(MessageHelper.Success(new DashboardViewModel
            {
                Connected = count.Connected,
                Pending = count.Pending
            }));
        }

        /// <summary>
        /// Get count of mentee and mentor of admin
        /// </summary>
        /// <param name="systemUserId">system user Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Dashboards/Admin")]
        public IActionResult Admin([FromRoute]int systemUserId)
        {
            var count = _connectionService.GetAdminCounts(systemUserId);
            return Ok(MessageHelper.Success(new AdminDashboardViewModel
            {
                Mentor = count.Mentor,
                Mentee = count.Mentee
            }));
        }

        ///// <summary>
        ///// Get What's Happening
        ///// </summary>
        ///// <param name="systemUserId">system user Id</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("WhatsHappening")]
        //public async Task<IActionResult> WhatsHappening([FromRoute]int systemUserId)
        //{
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

        //    var myNotif = await _dashboardService.Get(systemUserId, baseUrl);
        //    return Ok(MessageHelper.Success(myNotif));
        //}
        /// <summary>
        /// Get What's Happening
        /// </summary>
        /// <param name="systemUserId">system user Id</param>
        /// <param name="limit">page limit</param>
        /// <param name="pageIndex">start from 0</param>
        /// <returns></returns>
        [HttpGet]
        [Route("WhatsHappening")]
        public async Task<IActionResult> WhatsHappening([FromRoute]int systemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

            var myNotif = await _dashboardService.Get(systemUserId, baseUrl, limit, pageIndex);
            return Ok(MessageHelper.Success(myNotif));
        }

    }

    ///// <inheritdoc />
    //[Produces("application/json")]
    //[Route("api/v{version:apiVersion}/Users/{systemUserId}")]
    //[Authorize]
    //[ApiVersion("2")]
    //public class DashboardV2Controller : Controller
    //{
    //    private readonly IDashboardService _dashboardService;

    //    /// <inheritdoc />
    //    public DashboardV2Controller(IConnectionService connectionService, IDashboardService dashboardService)
    //    {
    //        _dashboardService = dashboardService;
    //    }


    //    /// <summary>
    //    /// Get What's Happening
    //    /// </summary>
    //    /// <param name="systemUserId">system user Id</param>
    //    /// <param name="limit">page limit</param>
    //    /// <param name="pageIndex">start from 0</param>
    //    /// <returns></returns>
    //    [HttpGet]
    //    [Route("WhatsHappening")]
    //    public async Task<IActionResult> WhatsHappening([FromRoute]int systemUserId, [FromQuery] int limit, [FromQuery] int pageIndex)
    //    {
    //        var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

    //        var myNotif = await _dashboardService.Get(systemUserId, baseUrl,limit,pageIndex);
    //        return Ok(MessageHelper.Success(myNotif));
    //    }
    //}
}