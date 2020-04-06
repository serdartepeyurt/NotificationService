namespace NotificationService.Core.Model
{
    using NotificationService.Core.Service.Abstract;
    using System.Collections.Generic;

    public class Notification
    {
        public string Id { get; set; }
        public NotificationType Type { get; set; }

        public string ProjectIdentifier { get; set; }

        public SendMethod Method { get; set; }
        public List<string> Target { get; set; }
        public List<string> CC { get; set; } = new List<string>();
        public List<string> BCC { get; set; } = new List<string>();

        public Platform TargetPlatform { get; set; }

        // EMAIL
        public string Message { get; set; }
        public string HtmlMessage { get; set; }
        public bool UseTemplate { get; set; }
        public EmailTemplateOptions TemplateOptions { get; set; }


        public string Title { get; set; }
        public string Action { get; set; }
        public int Badge { get; set; }

        public string CustomPayload { get; set; }

        // Hide default notification constructor
        private Notification() { }

        public static Notification CreateEmailNotification(string to, string title, string message, string projectIdentifier, string htmlMessage = null, bool usesTemplate = false, EmailTemplateOptions templateOptions = null)
        {
            var not = new Notification()
            {
                ProjectIdentifier = projectIdentifier,
                Title = title,
                Message = message,
                Type = NotificationType.Email,
                Target = new List<string> { to },
                HtmlMessage = htmlMessage,
                UseTemplate = usesTemplate,
                TemplateOptions = templateOptions
            };

            return not;
        }

        public static Notification CreateSmsNotification(string to, string message, string projectIdentifier)
        {
            var not = new Notification()
            {
                ProjectIdentifier = projectIdentifier,
                Message = message,
                Type = NotificationType.SMS,
                Target = new List<string> { to }
            };

            return not;
        }

        public static Notification CreatePushNotification(string to, string title, string message, Platform targetPlatform, string projectIdentifier, int badge = 0, string action = null, string customPayload = null)
        {
            var not = new Notification()
            {
                Title = title,
                Message = message,
                Type = NotificationType.Push,
                Target = new List<string> { to },
                TargetPlatform = targetPlatform,
                Badge = badge,
                Action = null,
                CustomPayload = null,
                ProjectIdentifier = projectIdentifier
            };

            return not;
        }
    }
}
