namespace NotificationService.Services
{
    using NotificationService.Core.Definitions;
    using NotificationService.Core.Model;
    using NotificationService.Core.Service.Abstract;
    using System.Net;
    using System.Net.Mail;

    /// <summary>
    /// SMTP email sender service
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private string _emailUser, _senderName, _password, _server;

        public SmtpEmailService(SmtpDefinition def)
        {
            this._senderName = def.SenderName;
            this._emailUser = def.EmailAddress;
            this._password = def.Password;
            this._server = def.SmtpServer;
            this.ProjectIdentifier = def.ProjectIdentifier;
        }

        public NotificationType Handles => NotificationType.Email;

        public string ProjectIdentifier { get; }

        public void SendNotification(Notification notification)
        {
            var client = new SmtpClient(this._server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this._emailUser, this._password)
            };

            MailMessage mailMessage = new MailMessage
            {
                Sender = new MailAddress(this._emailUser, this._senderName),
                From = new MailAddress(this._emailUser, this._senderName),
                Body = notification.Message,
                Subject = notification.Title
            };

            if (!string.IsNullOrEmpty(notification.HtmlMessage))
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(notification.HtmlMessage);
                htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                mailMessage.AlternateViews.Add(htmlView);
            }

            mailMessage.To.Add(notification.Target);
            client.Send(mailMessage);
        }
    }
}
