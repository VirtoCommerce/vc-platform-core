using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SitemapsModule.Core.Models;
using VirtoCommerce.SitemapsModule.Core.Models.Search;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapService
    {
        Task<Sitemap> GetByIdAsync(string sitemapId, string responseGroup = null);
        Task<IEnumerable<Sitemap>> GetByIdsAsync(string[] sitemapIds, string responseGroup = null);

        Task SaveChangesAsync(Sitemap[] sitemaps);

        Task RemoveAsync(string[] sitemapIds);
    }
}
