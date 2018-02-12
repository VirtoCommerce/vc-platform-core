namespace VirtoCommerce.NotificationsModule.Core.Abstractions
{
    public interface ITemplateRender
    {
        string Render(string template, object context);
    }
}
