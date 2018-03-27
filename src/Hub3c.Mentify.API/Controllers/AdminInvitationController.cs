using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/AdminInvitations")]
    [ApiVersion("1")]
    public class AdminInvitationsController : Controller
    {

        IAdminInvitationService _adminInvitationService;

        /// <inheritdoc />
        public AdminInvitationsController(IAdminInvitationService adminInvitationService)
        {
            _adminInvitationService = adminInvitationService;
        }

        /// <summary>
        /// Send email invitation as an admin  
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody] AdminInvitationModel model)
        {
            _adminInvitationService.Send(model);
            return Ok(MessageHelper.Success("The invitation emails have been sent"));
        }
    }
}