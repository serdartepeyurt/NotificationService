namespace NotificationService.Core.Definitions
{
    public class SmtpDefinition
    {
        public string ProjectIdentifier { get; set; }
        public string SmtpServer { get; set; }
        public string SenderName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int Port { get; set; }
    }
}
