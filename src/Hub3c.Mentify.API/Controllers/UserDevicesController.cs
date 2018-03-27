using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/UserDevices")]
    [ApiVersion("1")]
    public class UserDevicesController : Controller
    {
        private readonly ISystemUserDeviceService _systemUserDeviceService;

        /// <inheritdoc />
        public UserDevicesController(ISystemUserDeviceService systemUserDeviceService)
        {
            _systemUserDeviceService = systemUserDeviceService;
        }

        /// <summary>
        /// Add Current User Device 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]SystemUserDeviceViewModel model)
        {
            _systemUserDeviceService.Create(new SystemUserDeviceModel
            {
                SystemUserId = model.SystemUserId,
                DeviceToken = model.DeviceToken
            });
            return Ok(MessageHelper.Success("The Device Token has been created"));
        }

        /// <summary>
        /// Remove  User Device 
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete([FromBody]SystemUserDeviceViewModel model)
        {
            _systemUserDeviceService.Delete(new SystemUserDeviceModel
            {
                SystemUserId = model.SystemUserId,
                DeviceToken = model.DeviceToken
            });
            return Ok(MessageHelper.Success("The Device Token has been deleted"));
        }
    }
}