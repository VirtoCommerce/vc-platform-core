namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface INotificationTemplateRender
    {
        string Render(string template, object context);
    }
}
