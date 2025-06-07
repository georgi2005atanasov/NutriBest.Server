namespace NutriBest.Server.Features.Notifications
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using NutriBest.Server.Features.Notifications.Models;

    public class NotificationsController : ApiController
    {
        private readonly INotificationService notificationService;

        public NotificationsController(INotificationService notificationService) 
            => this.notificationService = notificationService;

        [HttpGet]
        [Authorize(Roles = "Administrator,Employee")]
        public async Task<ActionResult<AllNotificationsServiceModel>> AllNotifications([FromQuery] int page = 1)
        {
            try
            {
                var notifications = await notificationService.All(page);

                return Ok(notifications);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator,Employee")]
        [Route("{message}")]
        public async Task<ActionResult<AllNotificationsServiceModel>> DeleteNotificaiton([FromRoute] string message)
        {
            try
            {
                var result = await notificationService.DeleteNotification(message);

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
