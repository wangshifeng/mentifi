using System;
using System.Linq;
using System.Threading.Tasks;
using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Connections")]
    [ApiVersion("1")]
    [Authorize]
    public class ConnectionsController : Controller
    {
        private readonly IConnectionService _connectionService;
        /// <inheritdoc />
        public ConnectionsController(ILogger<ConnectionsController> logger, IConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        /// <summary>
        /// Request, accept, reject, cancel, remove a connection
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]  CreatedConnectionViewModel model)
        {
            switch (model.ConnectionPostType)
            {
                case ConnectionPostType.Request:
                    await _connectionService.Request(model.SenderSystemUserId, model.ReceiverSystemUserId, model.Message);
                    return Ok(MessageHelper.Success("The connection has been created successfully"));

                case ConnectionPostType.Accept:
                    await _connectionService.Accept(model.SenderSystemUserId, model.ReceiverSystemUserId);
                    return Ok(MessageHelper.Success("The connection has been accepted successfully"));

                case ConnectionPostType.Reject:
                    await _connectionService.Reject(model.SenderSystemUserId, model.ReceiverSystemUserId, model.Message);
                    return Ok(MessageHelper.Success("The connection has been rejected successfully"));

                case ConnectionPostType.Cancel:
                    _connectionService.Cancel(model.SenderSystemUserId, model.ReceiverSystemUserId);
                    return Ok(MessageHelper.Success("The connection request has been cancelled successfully"));

                case ConnectionPostType.Remove:
                    _connectionService.Remove(model.SenderSystemUserId, model.ReceiverSystemUserId);
                    return Ok(MessageHelper.Success("The connection  has been removed successfully"));
            }
            throw new ApplicationException("The ConnectionPostType is invalid");
        }

        /// <summary>
        /// Get Connection Status
        /// </summary>
        /// <param name="fromSystemUserId"></param>
        /// <param name="toSystemUserId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Status")]
        [ProducesResponseType(typeof(LookupModel<int>), 200)]
        public IActionResult Status([FromQuery] int fromSystemUserId, [FromQuery] int toSystemUserId)
        {
            return Ok(MessageHelper.Success(_connectionService.GetStatus(fromSystemUserId, toSystemUserId)));
        }
    }
}
