namespace NotificationService.Services
{
    using Microsoft.AspNetCore.NodeServices;
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
        private readonly INodeServices _nodeServices;
        private string _emailUser, _senderName, _password, _server;
        private int _port;
        private bool _useSSL;

        public SmtpEmailService(SmtpDefinition def, INodeServices nodeServices)
        {
            this._senderName = def.SenderName;
            this._emailUser = def.EmailAddress;
            this._password = def.Password;
            this._server = def.SmtpServer;
            this._port = def.Port;
            this._useSSL = def.UseSSL;
            this.ProjectIdentifier = def.ProjectIdentifier;

            this._nodeServices = nodeServices;
        }

        public NotificationType Handles => NotificationType.Email;

        public string ProjectIdentifier { get; }

        public void SendNotification(Notification notification)
        {
            using (var client = new SmtpClient(this._server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(this._emailUser, this._password),
                EnableSsl = this._useSSL,
                Port = this._port
            }){

                var templatedHtml = string.Empty;
                if (notification.UseTemplate)
                {
                    templatedHtml = _nodeServices.InvokeAsync<string>("./Templating/index", notification.TemplateOptions).Result;
                }

                MailMessage mailMessage = new MailMessage
                {
                    Sender = new MailAddress(this._emailUser, this._senderName),
                    From = new MailAddress(this._emailUser, this._senderName),
                    Body = notification.Message,
                    Subject = notification.Title
                };

                if (!string.IsNullOrEmpty(notification.HtmlMessage))
                {
                    if (notification.UseTemplate)
                    {
                        var htmlView = AlternateView.CreateAlternateViewFromString(templatedHtml);
                        htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                        mailMessage.AlternateViews.Add(htmlView);
                    }
                    else
                    {
                        var htmlView = AlternateView.CreateAlternateViewFromString(notification.HtmlMessage);
                        htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                        mailMessage.AlternateViews.Add(htmlView);
                    }
                }

                if(notification.CC.Count > 0)
                {
                    mailMessage.CC.Add(string.Join(", ", notification.CC));
                }

                if (notification.BCC.Count > 0)
                {
                    mailMessage.Bcc.Add(string.Join(", ", notification.BCC));
                }

                if (notification.Method == SendMethod.SINGLE)
                {
                    notification.Target.ForEach(t =>
                    {
                        mailMessage.To.Add(t);
                        client.Send(mailMessage);
                        mailMessage.To.Clear();
                    });
                }
                else
                {
                    mailMessage.To.Add(string.Join(", ", notification.Target));
                    client.Send(mailMessage);
                }
                
            }
        }
    }
}
