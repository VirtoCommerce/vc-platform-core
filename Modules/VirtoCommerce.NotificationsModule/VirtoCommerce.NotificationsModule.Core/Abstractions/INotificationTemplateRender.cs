namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    /// <summary>
    /// Rendering text by Liquid Templates 
    /// </summary>
    public interface INotificationTemplateRender
    {
        string Render(string template, object data);
    }
}
