using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Data.Models;

namespace VirtoCommerce.SitemapsModule.Data.Repositories
{
    public interface ISitemapRepository : IRepository
    {
        IQueryable<SitemapEntity> Sitemaps { get; }
        IQueryable<SitemapItemEntity> SitemapItems { get; }

        Task<IEnumerable<SitemapEntity>> GetSitemapsAsync(IEnumerable<string> ids, string responseGroup = null);
        Task<IEnumerable<SitemapItemEntity>> GetSitemapItemsAsync(IEnumerable<string> ids);
    }
}
