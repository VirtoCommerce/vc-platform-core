using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Data.Models;

namespace VirtoCommerce.SitemapsModule.Data.Repositories
{
    public interface ISitemapRepository : IRepository
    {
        IQueryable<SitemapEntity> Sitemaps { get; }

        IQueryable<SitemapItemEntity> SitemapItems { get; }
    }
}