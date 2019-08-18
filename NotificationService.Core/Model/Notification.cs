namespace NotificationService.Core.Model
{
    using NotificationService.Core.Service.Abstract;

    public class Notification
    {
        public string Id { get; set; }
        public NotificationType Type { get; set; }

        public string ProjectIdentifier { get; set; }

        public string Target { get; set; }
        public Platform TargetPlatform { get; set; }

        public string Message { get; set; }
        public string HtmlMessage { get; set; }
        public string Title { get; set; }
        public string Action { get; set; }
        public int Badge { get; set; }

        public string CustomPayload { get; set; }

        // Hide default notification constructor
        private Notification() { }

        public static Notification CreateEmailNotification(string to, string title, string message, string projectIdentifier, string htmlMessage = null)
        {
            var not = new Notification()
            {
                ProjectIdentifier = projectIdentifier,
                Title = title,
                Message = message,
                Type = NotificationType.Email,
                Target = to,
                HtmlMessage = htmlMessage
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
                Target = to
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
                Target = to,
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
