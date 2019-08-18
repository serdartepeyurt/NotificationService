namespace NotificationService.Client
{
    using Newtonsoft.Json;
    using NotificationService.Core.Client;
    using NotificationService.Core.Model;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class NotificationServiceClient : INotificationServiceClient
    {
        private readonly string _apiUrl;
        public NotificationServiceClient(string apiUrl)
        {
            this._apiUrl = apiUrl;
        }

        public async Task<Notification> SendAsync(Notification notification)
        {
            using (var httpClient = new HttpClient())
            {
                var resp = await httpClient.PostAsync($"{this._apiUrl}/SendNotification", new StringContent(JsonConvert.SerializeObject(notification), Encoding.UTF8, "application/json"));
                resp.EnsureSuccessStatusCode();

                var resultStr = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Notification>(resultStr);
                return result;
            }
        }
    }
}
