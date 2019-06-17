using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class SeoBySlugResolver : ISeoBySlugResolver
    {
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<ICatalogRepository> _repositoryFactory;
        public SeoBySlugResolver(Func<ICatalogRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }
        #region ISeoBySlugResolver members
        public async Task<SeoInfo[]> FindSeoBySlugAsync(string slug)
        {
            var result = new List<SeoInfo>();
            using (var repository = _repositoryFactory())
            {
                // Find seo entries for specified keyword. Also add other seo entries related to found object.
                //TODO: add caching
                result = (await repository.SeoInfos.Where(x => x.Keyword == slug)
                                                           .Join(repository.SeoInfos, x => new { x.ItemId, x.CategoryId }, y => new { y.ItemId, y.CategoryId }, (x, y) => y)
                                                           .ToArrayAsync()).Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();
            }
            return result.ToArray();
        }
        #endregion
    }
}
