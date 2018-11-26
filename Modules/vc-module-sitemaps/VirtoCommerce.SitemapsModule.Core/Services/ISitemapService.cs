using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapService
    {
        Task<Sitemap> GetByIdAsync(string sitemapId, string responseGroup = null);
        Task<IEnumerable<Sitemap>> GetByIdsAsync(string[] sitemapIds, string responseGroup = null);

        Task<GenericSearchResult<Sitemap>> SearchAsync(SitemapSearchCriteria criteria);

        Task SaveChangesAsync(Sitemap[] sitemaps);

        Task RemoveAsync(string[] sitemapIds);
    }
}
