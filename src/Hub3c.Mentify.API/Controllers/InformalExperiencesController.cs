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
    [Route("api/v{version:apiVersion}/Users/{systemUserId}/InformalExperiences")]
    [ApiVersion("1")]
    public class InformalExperiencesController : Controller
    {
        private readonly IInformalExperienceService _informalExperienceService;

        /// <inheritdoc />
        public InformalExperiencesController(IInformalExperienceService informalExperienceService)
        {
            _informalExperienceService = informalExperienceService;
        }

        /// <summary>
        /// Update informal experiences 
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateOrUpdate([FromRoute]int systemUserId, [FromBody]IEnumerable<InformalExperienceModel> model)
        {
            var informalExperience = new UserInformalExperienceModel()
            {
                InformalExperiences = model,
                SystemUserId = systemUserId
            };
            _informalExperienceService.CreateOrUpdate(informalExperience);
            return Ok(MessageHelper.Success("The informal experiences have been updated."));
        }
    }
}