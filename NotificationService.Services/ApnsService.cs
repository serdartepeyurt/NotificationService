namespace NotificationService.Services
{
    using Newtonsoft.Json;
    using NotificationService.Core.Definitions;
    using NotificationService.Core.Model;
    using NotificationService.Core.Service.Abstract;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    /// <summary>
    /// Apple push notification system service
    /// </summary>
    public class ApnsService : IPushNotificationService
    {
        private X509Certificate2Collection _certCollection;

        public Platform Platform => Platform.iOS;
        public NotificationType Handles => NotificationType.Push;

        public string ProjectIdentifier { get; }

        public ApnsService(ApnsDefinition def)
        {
            this.ProjectIdentifier = def.ProjectIdentifier;
            X509Certificate2 clientCertificate = new X509Certificate2(File.ReadAllBytes(def.ApnsCertPath), def.ApnsCertPass);
            this._certCollection = new X509Certificate2Collection(clientCertificate);
        }

        public void SendNotification(Notification notification)
        {
            var port = 2195;
            var hostname = "gateway.push.apple.com";

            if (this._certCollection[0].Subject.Contains("Apple Development IOS Push Services", StringComparison.InvariantCultureIgnoreCase))
            {
                hostname = "gateway.sandbox.push.apple.com";
            }

            foreach (var target in notification.Target)
            {
                TcpClient client = new TcpClient(AddressFamily.InterNetwork);
                client.Connect(hostname, port);

                SslStream sslStream = new SslStream(
                    client.GetStream(), false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null);

                sslStream.AuthenticateAsClient(hostname, this._certCollection, SslProtocols.Tls, false);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                /// Supports only single device notifications right now
                writer.Write(HexStringToByteArray(target.ToUpperInvariant()));
                string payload = ApsGenerator(notification);
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Dispose();
            }
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        private static string ApsGenerator(Notification notification)
        {
            // Check and return if custom payload applied to notification.
            if (!string.IsNullOrEmpty(notification.CustomPayload))
            {
                return notification.CustomPayload;
            }

            dynamic alert = string.IsNullOrEmpty(notification.Title) ? (dynamic)notification.Message : new
            {
                title = notification.Title,
                body = notification.Message
            };

            dynamic apsObj = new
            {
                aps = new
                {
                    alert,
                    badge = notification.Badge,
                    sound = "default"
                }
            };

            if (!string.IsNullOrEmpty(notification.Action))
            {
                apsObj.action = notification.Action;
            }

            return JsonConvert.SerializeObject(apsObj);
        }
    }
}
