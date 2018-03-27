using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/Educations")]
    [ApiVersion("1")]
    public class EducationsController : Controller
    {
        private readonly IEducationService _educationService;

        /// <inheritdoc />
        public EducationsController(IEducationService educationService)
        {
            _educationService = educationService;
        }

        /// <summary>
        /// Edit an education 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public ActionResult Delete([FromRoute]int systemUserId, [FromRoute]int id)
        {
            _educationService.Delete(systemUserId, id);
            return Ok(MessageHelper.Success("The education has been deleted."));
        }

        /// <summary>
        /// Edit an education 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult Edit([FromRoute]int systemUserId, [FromBody]EditedEducationViewModel model)
        {
            var education = new EditedUserEducationModel()
            {
                DateAttendedEnd = model.DateAttendedEnd,
                DateAttendedStart = model.DateAttendedStart,
                Degree = model.Degree,
                Id = model.Id,
                IsCurrentEducation = model.IsCurrentEducation,
                ModeOfStudy = model.ModeOfStudy,
                School = model.School,
                SystemUserId = systemUserId
            };
            _educationService.Edit(education);
            return Ok(MessageHelper.Success("The education has been updated."));
        }

        /// <summary>
        /// Create an education 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create([FromRoute]int systemUserId, [FromBody]NewEducationViewModel model)
        {
            var education = new UserEducationModel()
            {
                DateAttendedEnd = model.DateAttendedEnd,
                DateAttendedStart = model.DateAttendedStart,
                Degree = model.Degree,
                IsCurrentEducation = model.IsCurrentEducation,
                ModeOfStudy = model.ModeOfStudy,
                School = model.School,
                SystemUserId = systemUserId
            };
            _educationService.Create(education);
            return Ok(MessageHelper.Success("The education have been created."));
        }
    }
}