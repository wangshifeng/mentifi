using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/PersonalGoal")]
    [ApiVersion("1")]
    public class PersonalGoalController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        /// <inheritdoc />
        public PersonalGoalController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Edit personal goal 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        public ActionResult Edit([FromBody]EditedPersonalGoal model)
        {
            _userProfileService.EditPersonalGoal(model);
            return Ok(MessageHelper.Success("The personal goal has been updated."));
        }
    }
}