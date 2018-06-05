namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// Rendering text by Templates 
    /// </summary>
    public interface INotificationTemplateRenderer
    {
        string Render(string template, object data);
    }
}
