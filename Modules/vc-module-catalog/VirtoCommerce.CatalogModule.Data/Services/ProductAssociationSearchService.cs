using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Caching;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class ProductAssociationSearchService : IProductAssociationSearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        public ProductAssociationSearchService(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService, IPlatformMemoryCache platformMemoryCache)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<GenericSearchResult<ProductAssociation>> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var cacheKey = CacheKey.With(GetType(), "SearchProductAssociationsAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(ItemSearchCacheRegion.CreateChangeToken());

                if (criteria.ObjectIds.IsNullOrEmpty()) return new GenericSearchResult<ProductAssociation>();

                using (var repository = _catalogRepositoryFactory())
                {
                    //Optimize performance and CPU usage
                    repository.DisableChangesTracking();

                    var result = new GenericSearchResult<ProductAssociation>();

                    var dbResult = await repository.SearchAssociations(criteria);

                    result.TotalCount = dbResult.TotalCount;
                    result.Results = dbResult.Results
                        .Select(x => x.ToModel(AbstractTypeFactory<ProductAssociation>.TryCreateInstance())).ToList();
                    return result;
                }
            });
        }
    }
}
