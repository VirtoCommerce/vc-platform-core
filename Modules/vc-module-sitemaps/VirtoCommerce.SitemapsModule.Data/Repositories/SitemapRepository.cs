using System.Linq;
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
    }
}
