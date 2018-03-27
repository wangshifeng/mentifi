using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/Experiences")]
    [ApiVersion("1")]
    public class ExperiencesController : Controller
    {
        private readonly IExperienceService _experienceService;

        /// <inheritdoc />
        public ExperiencesController(IExperienceService experienceService)
        {
            _experienceService = experienceService;
        }

        /// <summary>
        /// Edit an experience 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete([FromRoute]int systemUserId, [FromRoute]int id)
        {
            _experienceService.Delete(systemUserId, id);
            return Ok(MessageHelper.Success("The experience has been deleted."));
        }

        /// <summary>
        /// Edit an experience 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult Edit([FromRoute]int systemUserId, [FromBody]EditedExperienceViewModel model)
        {
            var experience = new EditedUserExperienceModel
            {
                CompanyName = model.CompanyName,
                EndMonth = model.EndMonth,
                EndYear = model.EndYear,
                Id = model.Id,
                StartMonth = model.StartMonth,
                StartYear = model.StartYear,
                Title = model.Title,
                SystemUserId = systemUserId,
                IsCurrentlyWorkHere = model.IsCurrentlyWorkHere,
                Resume = model.Resume
            };
            _experienceService.Edit(experience);
            return Ok(MessageHelper.Success("The experience has been updated."));
        }

        /// <summary>
        /// Create an experience 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([FromRoute]int systemUserId, [FromBody]NewExperienceViewModel model)
        {
            var experience = new UserExperienceModel
            {
                CompanyName = model.CompanyName,
                EndMonth = model.EndMonth,
                EndYear = model.EndYear,
                StartMonth = model.StartMonth,
                StartYear = model.StartYear,
                Title = model.Title,
                SystemUserId = systemUserId,
                IsCurrentlyWorkHere = model.IsCurrentlyWorkHere,
                Resume = model.Resume
            };
            _experienceService.Create(experience);
            return Ok(MessageHelper.Success("The experience have been created."));
        }
    }
}