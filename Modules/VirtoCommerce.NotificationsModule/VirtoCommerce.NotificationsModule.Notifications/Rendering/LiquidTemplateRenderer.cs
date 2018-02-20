using Scriban;

namespace VirtoCommerce.NotificationsModule.Notifications.Rendering
{
    public class LiquidTemplateRenderer : INotificationTemplateRender
    {
        public string Render(string stringTemplate, object context)
        {
            var template = Template.ParseLiquid(stringTemplate);
            var result = template.Render(context);
            return result;
        }
    }
}
