using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapUrlBuilder
    {
        string BuildStoreUrl(Store store, string language, string urlTemplate, string baseUrl, IEntity entity = null);
    }
}
