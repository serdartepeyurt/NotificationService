namespace NotificationService.Core.Service.Abstract
{
    public interface IPushNotificationService : INotificationService
    {
        Platform Platform { get; }
    }

    public enum Platform
    {
        iOS,
        Android,
        Web
    }
}
