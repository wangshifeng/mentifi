using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.Service;
using Hub3c.Mentify.Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/MessageBoards")]
    [ApiVersion("1")]
    [Authorize]
    public class MessageBoardsController : Controller
    {
        private readonly IMessageBoardService _messageBoardService;

        /// <inheritdoc />
        public MessageBoardsController(IMessageBoardService messageBoardService)
        {
            _messageBoardService = messageBoardService;
        }

        /// <summary>
        /// Get Message Boards
        /// </summary>
        /// <param name="fromSystemUserId"></param>
        /// <param name="toSystemUserId"></param>
        /// <param name="limit"></param>
        /// <param name="pageIndex">start from 0</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(MessageBoardModel), 200)]
        public IActionResult Get([FromQuery]int fromSystemUserId, [FromQuery]int toSystemUserId, int limit, int pageIndex)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var results = _messageBoardService.Get(fromSystemUserId, toSystemUserId, baseUrl, limit, pageIndex);
            return Ok(MessageHelper.Success(results));
        }

        /// <summary>
        /// Create a Message Board
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]NewMessageBoardModel model)
        {
            _messageBoardService.Create(model);
            return Ok(MessageHelper.Success("The message has been sent."));
        }
    }
}