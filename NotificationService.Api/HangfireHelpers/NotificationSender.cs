namespace NotificationService.Api.HangfireHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NotificationService.Core.Model;
    using NotificationService.Core.Service.Abstract;

    public class NotificationSender
    {
        private readonly IEnumerable<INotificationService> notificationServices;

        public NotificationSender(IEnumerable<INotificationService> notificationServices)
        {
            this.notificationServices = notificationServices;
        }

        public void SendNotification(Notification notification)
        {
            INotificationService nService = null;

            switch (notification.Type)
            {
                case NotificationType.Email:
                    nService = this.notificationServices.FirstOrDefault(ns => ns is IEmailService);
                    break;
                case NotificationType.SMS:
                    nService = this.notificationServices.FirstOrDefault(ns => ns is ISmsService);
                    break;
                case NotificationType.Push:
                    var services = this.notificationServices.Where(ns => (ns is IPushNotificationService) && ((IPushNotificationService)ns).ProjectIdentifier == notification.ProjectIdentifier).Select(ns => ns as IPushNotificationService);
                    nService = services.FirstOrDefault(ns => ns.Platform == notification.TargetPlatform);
                    break;
            }

            if (nService == null)
            {
                throw new ArgumentNullException(notification.Type.ToString(), "There is no such service defined");
            }

            nService.SendNotification(notification);
        }
    }
}
