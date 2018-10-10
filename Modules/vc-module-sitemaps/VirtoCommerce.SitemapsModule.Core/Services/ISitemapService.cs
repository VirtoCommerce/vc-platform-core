using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapService
    {
        Sitemap GetById(string id);

        GenericSearchResult<Sitemap> Search(SitemapSearchCriteria criteria);

        void SaveChanges(Sitemap[] sitemaps);

        void Remove(string[] sitemapIds);
    }
}
