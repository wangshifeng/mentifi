using System.Threading.Tasks;
using Hub3c.Mentify.AccessInternalApi;
using Hub3c.Mentify.AccessInternalApi.Models;
using Hub3c.Mentify.API.Helpers;
using Hub3c.Mentify.API.Models;
using Hub3c.Mentify.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hub3c.Mentify.API.Controllers
{
    /// <inheritdoc />
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/Notifications")]
    [Authorize]
    [ApiVersion("1")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IHub3cFirebaseApi _hub3CFirebaseApi;
        /// <inheritdoc />
        public NotificationsController(INotificationService notificationService, IHub3cFirebaseApi hub3CFirebaseApi)
        {
            _notificationService = notificationService;
            _hub3CFirebaseApi = hub3CFirebaseApi;
        }

        /// <summary>
        /// Clear notification by id
        /// </summary>
        /// <param name="notificationId"> Notification ID</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("NotificationId")]
        public IActionResult ById([FromBody]  int notificationId)
        {
            _notificationService.Update(notificationId);
            return Ok(MessageHelper.Success("The notification has been cleared"));
        }

        /// <summary>
        ///  Clear notification by type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("NotificationType")]
        [HttpPatch]
        public IActionResult ByType([FromBody]EditedNotificationViewModel model)
        {
            _notificationService.Update(model.SystemUserId, model.NotificationType);
            return Ok(MessageHelper.Success("The notification has been cleared"));
        }

        /// <summary>
        /// Clear all notifications
        /// </summary>
        /// <param name="systemUserId"></param>
        /// <returns></returns>
        [Route("{systemUserId}")]
        [HttpPatch]
        public IActionResult UpdateAll([FromRoute]int systemUserId)
        {
            _notificationService.UpdateAll(systemUserId);
            return Ok(MessageHelper.Success("The all notifications has been cleared"));
        }


        [Route("Test")]
        [HttpPatch]
        public async Task<IActionResult> Test()
        {
            var result = await _hub3CFirebaseApi.Send(new Hub3cFirebase()
            {
                Notification = new FIrebaseNotification()
                {
                    Title = "Mentifi",
                    Body =
                        "Garry Nilson has sent you a < a href =\"../../mentifi/network\">connection request</a>, communicate further in <a href=\"javascript:void(0)\" onclick=\"checkAvailableMessageBoardLinks('{0}', '{1}')\">message board</a>",
                    Icon = "default",
                    Sound = "default",
                    Badge = 1,
                    mutable_content = true,
                    Click_Action = NotificationType.Pending,
                },
                To =
                    "ejEDLXuaMJw:APA91bF5HNWcJSZGny5JY53BRaHAEMJoMxLzz-pZnbs6iXjaXE6FCqApR3NUBCNA8kccUyZByizp9f14x1ZGrI_P89mms58FBCwJtVBFH9SLh4aGUrdxYbguFOPDllxyRg8PZXotcJqF"
            });
            return Ok(MessageHelper.Success(result));
        }
    }
}