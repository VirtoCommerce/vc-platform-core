using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
