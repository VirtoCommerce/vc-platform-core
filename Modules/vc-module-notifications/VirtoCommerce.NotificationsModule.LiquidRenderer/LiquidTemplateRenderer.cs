using Scriban;
using VirtoCommerce.NotificationsModule.Core.Services;

namespace VirtoCommerce.NotificationsModule.LiquidRenderer
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
