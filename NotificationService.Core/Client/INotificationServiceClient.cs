namespace NotificationService.Core.Client
{
    using Model;
    using System.Threading.Tasks;

    public interface INotificationServiceClient
    {
        Task<Notification> SendAsync(Notification notification);
    }
}
