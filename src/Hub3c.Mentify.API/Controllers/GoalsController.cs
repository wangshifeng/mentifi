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
    [Route("api/v{version:apiVersion}/Goals")]
    [Authorize]
    [ApiVersion("1")]
    public class GoalsController : Controller
    {
        private readonly IGoalService _goalService;

        /// <inheritdoc />
        public GoalsController(IGoalService goalService)
        {
            _goalService = goalService;
        }

        /// <summary>
        /// Get Goals
        /// </summary>
        /// <param name="systemUserId">system user Id</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery]int systemUserId)
        {
            //var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var goals = _goalService.GetBySystemUser(systemUserId);
            return Ok(MessageHelper.Success(goals));
        }

        /// <summary>
        /// Get Goals
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]NewGoalModel model)
        {
            _goalService.Create(model);
            return Ok(MessageHelper.Success("The goal progress has been created."));
        }


        /// <summary>
        /// Edit a Goal
        /// </summary>
        /// <param name="goalId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{goalId}")]
        public IActionResult Put([FromRoute] int goalId, [FromBody]EditedGoalViewModel model)
        {
            _goalService.Edit(new EditedGoalModel()
            {
                Id = goalId,
                MenteeSystemUserId = model.MenteeSystemUserId,
                Description = model.Description,
                ProbabilityId = model.ProbabilityId
            });
            return Ok(MessageHelper.Success("Goal progress is successfully updated."));
        }

        /// <summary>
        /// Delete a Goal
        /// </summary>
        /// <param name="goalId"></param>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{goalId}/SystemUsers/{systemUserId}")]
        public IActionResult Delete([FromRoute] int goalId, [FromRoute]int systemUserId)
        {
            _goalService.Delete(goalId, systemUserId);
            return Ok(MessageHelper.Success("Successfully removed the goal."));
        }
    }
}