using System.Collections.Generic;

namespace NotificationService.Core.Model
{
    public class EmailTemplateOptions
    {
        public string LogoUrl { get; set; }
        public string FirmWebSite { get; set; }
        public string FirmName { get; set; }
        public string FirmAddress { get; set; }
        public string FirmPhone { get; set; }
        public string FirmEmail { get; set; }
        public string PrimaryColor { get; set; }
        public string AccentColor { get; set; }
        public string PreHeader { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<LinkObject> Buttons { get; set; }
        public List<LinkObject> FooterLinks { get; set; }
    }
}