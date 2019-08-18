namespace NotificationService.Services
{
    using NotificationService.Core.Model;
    using NotificationService.Core.Service.Abstract;

    public class NetGsmSmsService : ISmsService
    {
        public NotificationType Handles => NotificationType.SMS;

        public string ProjectIdentifier => throw new System.NotImplementedException();

        public void SendNotification(Notification notification)
        {
            throw new System.NotImplementedException();
        }
    }
}
