using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Data.Infrastructure;
using VirtoCommerce.SitemapsModule.Data.Models;

namespace VirtoCommerce.SitemapsModule.Data.Repositories
{
    public class SitemapRepository : DbContextRepositoryBase<SitemapDbContext>, ISitemapRepository
    {
        public SitemapRepository(SitemapDbContext dbContext, IUnitOfWork unitOfWork = null)
            : base(dbContext, unitOfWork)
        {
        }

        public IQueryable<SitemapEntity> Sitemaps => DbContext.Set<SitemapEntity>();
        public IQueryable<SitemapItemEntity> SitemapItems => DbContext.Set<SitemapItemEntity>();

        public async Task<IEnumerable<SitemapEntity>> GetSitemapsAsync(IEnumerable<string> ids, string responseGroup = null)
        {
            var result = await Sitemaps.Where(x => ids.Contains(x.Id)).ToListAsync();
            var totalItemsCounts = await SitemapItems.Where(x => ids.Contains(x.SitemapId)).GroupBy(x => x.SitemapId)
                                                   .Select(x => new { SitemapId = x.Key, TotalItemsCount = x.Count() }).ToArrayAsync();
            foreach (var totalItemsCount in totalItemsCounts)
            {
                var sitemap = result.FirstOrDefault(x => x.Id.EqualsInvariant(totalItemsCount.SitemapId));
                if (sitemap != null)
                {
                    sitemap.TotalItemsCount = totalItemsCount.TotalItemsCount;
                }
            }
            return result;

        }
        public async Task<IEnumerable<SitemapItemEntity>> GetSitemapItemsAsync(IEnumerable<string> ids)
        {
            var result = await SitemapItems.Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }
    }
}
