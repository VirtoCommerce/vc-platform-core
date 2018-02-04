using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.NotificationsModule.Web.ViewModels
{
    public class TemplateResult
    {
        public string Id { get; set; }
        public string NotificationType { get; set; }
        public string Language { get; set; }
        public bool IsDefault { get; set; }
        public string Created { get; set; }
        public string Modified { get; set; }
        public string DisplayName { get; set; }
        public string SendGatewayType { get; set; }
        public object CcRecipients { get; set; }
        public object BccRecipients { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string DynamicProperties { get; set; }
    }
}
