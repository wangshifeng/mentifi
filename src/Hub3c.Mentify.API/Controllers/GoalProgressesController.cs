using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Goals/{goalId}/Progresses")]
    [Authorize]
    [ApiVersion("1")]
    public class GoalProgressesController : Controller
    {
        private readonly IGoalProgressService _goalProgressService;

        /// <inheritdoc />
        public GoalProgressesController(IGoalProgressService goalProgressService)
        {
            _goalProgressService = goalProgressService;
        }


        /// <summary>
        /// Get Goals
        /// </summary>
        /// <param name="goalId">Goal Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromRoute] int goalId, [FromBody]NewGoalProgressViewModel model)
        {
            _goalProgressService.Create(new NewGoalProgressModel
            {
                MenteeSystemUserId = model.MenteeSystemUserId,
                Reason = model.Reason,
                ProgressValue = model.ProgressValue,
                GoalId = goalId
            });
            return Ok(MessageHelper.Success("Goal progress is successfully updated."));
        }
    }
}