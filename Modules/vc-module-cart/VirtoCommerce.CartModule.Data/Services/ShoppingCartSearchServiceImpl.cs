using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Caching;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartSearchServiceImpl : IShoppingCartSearchService
    {
        private readonly Func<ICartRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public ShoppingCartSearchServiceImpl(Func<ICartRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<GenericSearchResult<ShoppingCart>> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<ShoppingCart>();
            var cacheKey = CacheKey.With(GetType(), "SearchCartAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CartSearchCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var query = repository.ShoppingCarts;

                    if (!string.IsNullOrEmpty(criteria.Status))
                    {
                        query = query.Where(x => x.Status == criteria.Status);
                    }

                    if (!string.IsNullOrEmpty(criteria.Name))
                    {
                        query = query.Where(x => x.Name == criteria.Name);
                    }

                    if (!string.IsNullOrEmpty(criteria.CustomerId))
                    {
                        query = query.Where(x => x.CustomerId == criteria.CustomerId);
                    }

                    if (!string.IsNullOrEmpty(criteria.StoreId))
                    {
                        query = query.Where(x => criteria.StoreId == x.StoreId);
                    }

                    if (!string.IsNullOrEmpty(criteria.Currency))
                    {
                        query = query.Where(x => x.Currency == criteria.Currency);
                    }

                    if (!string.IsNullOrEmpty(criteria.Type))
                    {
                        query = query.Where(x => x.Type == criteria.Type);
                    }

                    if (!string.IsNullOrEmpty(criteria.OrganizationId))
                    {
                        query = query.Where(x => x.OrganizationId == criteria.OrganizationId);
                    }

                    if (!criteria.CustomerIds.IsNullOrEmpty())
                    {
                        query = query.Where(x => criteria.CustomerIds.Contains(x.CustomerId));
                    }

                    var sortInfos = criteria.SortInfos;
                    if (sortInfos.IsNullOrEmpty())
                    {
                        sortInfos = new[]
                        {
                            new SortInfo
                            {
                                SortColumn = ReflectionUtility.GetPropertyName<ShoppingCartEntity>(x => x.CreatedDate),
                                SortDirection = SortDirection.Descending
                            }
                        };
                    }

                    query = query.OrderBySortInfos(sortInfos);

                    retVal.TotalCount = await query.CountAsync();

                    var carts = await query.Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    retVal.Results = carts.Select(x => x.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance())).ToList();

                    return retVal;
                }
            });
        }
    }
}
