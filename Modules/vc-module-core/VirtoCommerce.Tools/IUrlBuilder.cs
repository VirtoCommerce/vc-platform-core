using VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Tools
{
    public interface IUrlBuilder
    {
        string BuildStoreUrl(UrlBuilderContext context, string virtualPath);
        string BuildStoreUrl(UrlBuilderContext context, string virtualPath, Store store, string language);
    }
}
