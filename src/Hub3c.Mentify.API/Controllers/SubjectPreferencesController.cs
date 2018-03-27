using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/SubjectPreferences")]
    [ApiVersion("1")]
    public class SubjectPreferencesController : Controller
    {
        private readonly ISubjectPreferenceService _subjectPreferenceService;

        /// <inheritdoc />
        public SubjectPreferencesController(ISubjectPreferenceService subjectPreferenceService)
        {
            _subjectPreferenceService = subjectPreferenceService;
        }

        /// <summary>
        /// Update subject preferences 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOrUpdate([FromRoute]int systemUserId, [FromBody]EduSubjectPreferenceViewModel model)
        {
            var informalExperience = new CreatedEduSubjectPreference()
            {
                FieldOfStudies = model.FieldOfStudies,
                SystemUserId = systemUserId,
                PreferredMenteeGrade = model.PreferredMenteeGrade
            };
            _subjectPreferenceService.CreateOrUpdate(informalExperience);
            return Ok(MessageHelper.Success("The subject preferences have been updated."));
        }
    }
}