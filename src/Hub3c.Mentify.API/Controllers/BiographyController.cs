using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Biography")]
    [ApiVersion("1")]
    public class BiographyController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        /// <inheritdoc />
        public BiographyController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Edit biography 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        public ActionResult Edit([FromBody]EditedBiography model)
        {
            _userProfileService.EditBiography(model);
            return Ok(MessageHelper.Success("The biography has been updated."));
        }
    }
}