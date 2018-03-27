using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Constant = Hub3c.Mentify.Service.Models.Constant;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Lookup")]
    [Authorize]
    [ApiVersion("1")]
    public class LookupController : Controller
    {
        private readonly ILookupService _lookupService;

        /// <inheritdoc />
        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        /// <summary>
        /// Get Subject Preferences
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SubjectPreferences")]
        public IActionResult SubjectPreferences()
        {
            var result = _lookupService.GetSubjectPreferences();
            return Ok(MessageHelper.Success(result));
        }

        /// <summary>
        /// Get Edu Grades
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("EduGrades")]
        public IActionResult EduGrades()
        {
            var result = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduGrade);
            return Ok(MessageHelper.Success(result));
        }

        /// <summary>
        /// Get Edu Additional Activities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AdditionalActivities")]
        public IActionResult AdditionalActivity()
        {
            var result = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduAdditionalActivity);
            return Ok(MessageHelper.Success(result));
        }

        /// <summary>
        /// Get Edu Informal Experiences
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("InformalExperiences")]
        public IActionResult InformalExperience()
        {
            var result = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduInformalExperience);
            return Ok(MessageHelper.Success(result));
        }

        /// <summary>
        /// Get Mode Of Study
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ModeOfStudy")]
        public IActionResult ModeOfStudy()
        {
            var result = _lookupService.GetByAttributeName(Constant.LookupTypeCode_EduModeOfStudy);
            return Ok(MessageHelper.Success(result));
        }

    }
}