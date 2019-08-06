using System;
using System.Collections.Generic;

namespace VirtoCommerce.NotificationsModule.Core.Model
{
    public class NotificationBuilder
    {
        public IList<NotificationTemplate> PredefinedTemplates = new List<NotificationTemplate>();

        public NotificationBuilder WithTemplates(params NotificationTemplate[] templates)
        {
            if (templates == null)
            {
                throw new ArgumentNullException(nameof(templates));
            }
            foreach (var template in templates)
            {
                template.IsReadonly = true;
                PredefinedTemplates.Add(template);
            }
            return this;
        }
    }
}
