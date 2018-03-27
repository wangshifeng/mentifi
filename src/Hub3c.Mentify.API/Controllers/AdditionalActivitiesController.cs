using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/AdditionalActivities")]
    [ApiVersion("1")]
    public class AdditionalActivitiesController : Controller
    {
        private readonly IAdditionalActivityService _additionalActivityService;

        /// <inheritdoc />
        public AdditionalActivitiesController(IAdditionalActivityService additionalActivityService)
        {
            _additionalActivityService = additionalActivityService;
        }

        /// <summary>
        /// Update additional activities 
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
            _additionalActivityService.CreateOrUpdate(informalExperience);
            return Ok(MessageHelper.Success("The additional activities have been updated."));
        }
    }
}