namespace NotificationService.Controllers
{
    using Hangfire;
    using Microsoft.AspNetCore.Mvc;
    using NotificationService.Api.HangfireHelpers;
    using NotificationService.Core.Model;

    [Route("api/[controller]")]
    [ApiController]
    public class SendNotificationController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendNotification(Notification notification)
        {
            notification.Id = BackgroundJob.Enqueue<NotificationSender>(ns => ns.SendNotification(notification));
            return Ok(notification);
        }
    }
}
