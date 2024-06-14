namespace NutriBest.Server.Features.Notifications
{
    using Microsoft.AspNetCore.Mvc;
    using NutriBest.Server.Features.Notifications.Models;

    public class NotificationsController : ApiController
    {
        private readonly INotificationService notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
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
    }
}
