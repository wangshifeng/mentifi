using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Profile")]
    [ApiVersion("1")]
    public class UserController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        /// <inheritdoc />
        public UserController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Get authenticated user profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(CurrentUser), 200)]
        public IActionResult Get()
        {
            var mid = User.GetMid();
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";


            var profile = _userProfileService.Get(mid, baseUrl);
            return Ok(MessageHelper.Success(profile));
        }

        /// <summary>
        /// Get authenticated user profile
        /// </summary>
        /// <param name="systemUserId">System User ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{systemUserId}")]
        [ProducesResponseType(typeof(UserProfileModel), 200)]
        public IActionResult Get([FromRoute]int systemUserId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";

            var profile = _userProfileService.GetBySystemUserId(systemUserId, baseUrl);
            return Ok(MessageHelper.Success(profile));
        }

        /// <summary>
        /// Get user setting
        /// </summary>
        /// <param name="systemUserId">System User ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{systemUserId}/Setting")]
        [ProducesResponseType(typeof(UserSettingModel), 200)]
        public IActionResult Setting([FromRoute]int systemUserId)
        {
            var profile = _userProfileService.GetUserSetting(systemUserId);
            return Ok(MessageHelper.Success(profile));
        }

        /// <summary>
        /// Get Mentifi Type
        /// </summary>
        /// <param name="businessId">Business ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("MentifiType/{businessId}")]
        [ProducesResponseType(typeof(LookupModel<int>), 200)]
        public IActionResult MentifiType([FromRoute]int businessId)
        {
            var mentifiType = _userProfileService.GetMentifiType(businessId);
            return Ok(MessageHelper.Success(mentifiType));
        }

        /// <summary>
        /// Edit user profile
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public IActionResult Edit([FromBody]EditedUserProfileModel model)
        {
            _userProfileService.Edit(model);
            return Ok(MessageHelper.Success("The profile has been updated."));
        }

    }
}
