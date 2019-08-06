using System.Threading.Tasks;

namespace VirtoCommerce.NotificationsModule.Core.Services
{
    /// <summary>
    /// Rendering text by Templates 
    /// </summary>
    public interface INotificationTemplateRenderer
    {
        Task<string> RenderAsync(string stringTemplate, object model, string language = null);
    }
}
