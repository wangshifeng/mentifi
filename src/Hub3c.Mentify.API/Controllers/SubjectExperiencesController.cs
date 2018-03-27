using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/SubjectExperiences")]
    [ApiVersion("1")]
    public class SubjectExperiencesController : Controller
    {
        private readonly ISubjectExperienceService _subjectExperienceService;

        /// <inheritdoc />
        public SubjectExperiencesController(ISubjectExperienceService subjectExperienceService)
        {
            _subjectExperienceService = subjectExperienceService;
        }

        /// <summary>
        /// Update informal experiences 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOrUpdate([FromRoute]int systemUserId, [FromBody]IEnumerable<CreatedFieldOfStudy> model)
        {
            var informalExperience = new UserFieldOfStudy()
            {
                FieldOfStudies = model,
                SystemUserId = systemUserId
            };
            _subjectExperienceService.CreateOrUpdate(informalExperience);
            return Ok(MessageHelper.Success("The subject experiences have been updated."));
        }
    }
}