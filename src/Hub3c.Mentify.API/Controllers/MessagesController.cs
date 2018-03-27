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
    [Route("api/v{version:apiVersion}/Messages")]
    [ApiVersion("1")]
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService _messageService;

        /// <inheritdoc />
        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Inbox
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <param name="pageIndex">start from 0</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Inbox")]
        [ProducesResponseType(typeof(PagedListModel<MessageModel>), 200)]
        public IActionResult Inbox([FromQuery]int systemUserId, [FromQuery] int pageIndex, [FromQuery] int limit)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var results = _messageService.GetInbox(systemUserId, pageIndex, limit, baseUrl);
            return Ok(MessageHelper.Success(results));
        }

        /// <summary>
        /// Set the message status as read
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("{messageId}/Read")]
        public IActionResult SetAsRead([FromRoute] int messageId)
        {
            _messageService.SetAsRead(messageId);
            return Ok(MessageHelper.Success("The message has been read."));
        }

        /// <summary>
        /// Create a message
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody] NewMessageModel model)
        {
            _messageService.Create(model);
            return Ok(MessageHelper.Success("The message has been created."));
        }


        /// <summary>
        /// Delete a message
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("{messageId}")]
        public IActionResult Delete([FromRoute] int messageId)
        {
            _messageService.Delete(messageId);
            return Ok(MessageHelper.Success("The message has been removed."));
        }

        /// <summary>
        /// Get  message detail
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{messageId}")]
        public IActionResult Detail([FromRoute] int messageId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            return Ok(MessageHelper.Success(_messageService.GetById(messageId, baseUrl)));
        }

        /// <summary>
        /// Outbox
        /// </summary>
        /// <param name="systemUserId"></param>
        /// /// <param name="pageIndex">start from 0</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Outbox")]
        [ProducesResponseType(typeof(PagedListModel<MessageModel>), 200)]
        public IActionResult Outbox([FromQuery]int systemUserId, [FromQuery]int pageIndex, [FromQuery]int limit)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/v{HttpContext.GetRequestedApiVersion().ToString()}";
            var results = _messageService.GetOutbox(systemUserId, pageIndex, limit, baseUrl);
            return Ok(MessageHelper.Success(results));
        }
    }
}