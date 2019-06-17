using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Data.Repositories;

namespace VirtoCommerce.StoreModule.Data.Services
{
    public class SeoBySlugResolver : ISeoBySlugResolver
    {
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly Func<IStoreRepository> _repositoryFactory;
        public SeoBySlugResolver(Func<IStoreRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
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
                                                           .Join(repository.SeoInfos, x => x.StoreId, y => y.StoreId, (x, y) => y)
                                                           .ToArrayAsync()).Select(x => x.ToModel(AbstractTypeFactory<SeoInfo>.TryCreateInstance())).ToList();
            }
            return result.ToArray();
        }
        #endregion
    }
}
