using System;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Photos")]
    [ApiVersion("1")]
    public class PhotosController : Controller
    {
        private readonly IUserProfileService _userProfileService;

        /// <inheritdoc />
        public PhotosController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Get User Photo
        /// </summary>
        /// <param name="id">System User Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public IActionResult UserPhoto([FromRoute]int id)
        {
            var data = _userProfileService.GetPhoto(id);
            if (data != null)
            {
                return File(data, "image/jpeg");
            }

            throw new ApplicationException("Photo profile is empty");
        }
    }
}