namespace NotificationService.Core.Service.Abstract
{
    using NotificationService.Core.Model;

    public interface INotificationService
    {
        string ProjectIdentifier { get; }
        NotificationType Handles { get; }
        void SendNotification(Notification notification);
    }
}
