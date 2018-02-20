namespace VirtoCommerce.NotificationsModule.Notifications.Rendering
{
    public interface INotificationTemplateRender
    {
        string Render(string template, object context);
    }
}
