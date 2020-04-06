namespace NotificationService.Services
{
    using Newtonsoft.Json;
    using NotificationService.Core.Definitions;
    using NotificationService.Core.Model;
    using NotificationService.Core.Service.Abstract;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Firebase cloud messaging service
    /// </summary>
    public class FcmService : IPushNotificationService
    {
        private string _serverKey;
        private string _senderId;

        public Platform Platform => Platform.Android;
        public NotificationType Handles => NotificationType.Push;

        public string ProjectIdentifier { get; }

        public FcmService(FcmDefinition def)
        {
            this.ProjectIdentifier = def.ProjectIdentifier;
            this._serverKey = def.FcmServerKey;
            this._senderId = def.FcmSenderId;
        }

        public void SendNotification(Notification notification)
        {
            foreach (var target in notification.Target)
            {
                var tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add($"Authorization: key={this._serverKey}");
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add($"Sender: id={this._senderId}");
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = target,
                    priority = "high",
                    content_available = true,
                    notification = new
                    {
                        body = notification.Message,
                        title = notification.Title,
                        badge = 0
                    },
                };

                string postbody = JsonConvert.SerializeObject(payload).ToString();
                var byteArray = Encoding.UTF8.GetBytes(postbody);
                tRequest.ContentLength = byteArray.Length;
                using (var dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (var dataStreamResponse = tResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null)
                            {
                                using (var tReader = new StreamReader(dataStreamResponse))
                                {
                                    var sResponseFromServer = tReader.ReadToEnd();
                                    if (sResponseFromServer.Contains("error"))
                                    {
                                        throw new System.Exception($"ERROR IN FCM Connection. Response: {sResponseFromServer}");
                                    }
                                    //result.Response = sResponseFromServer;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}