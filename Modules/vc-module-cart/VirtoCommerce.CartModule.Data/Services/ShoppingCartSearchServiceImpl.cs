using System;
using System.Collections.Generic;
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
        public ShoppingCartSearchServiceImpl(Func<ICartRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IShoppingCartService cartService)
        {
            RepositoryFactory = repositoryFactory;
            PlatformMemoryCache = platformMemoryCache;
            CartService = cartService;
        }

        protected Func<ICartRepository> RepositoryFactory { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }
        protected IShoppingCartService CartService { get; }

        public async Task<GenericSearchResult<ShoppingCart>> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            var retVal = new GenericSearchResult<ShoppingCart>();
            var cacheKey = CacheKey.With(GetType(), "SearchCartAsync", criteria.GetCacheKey());
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(CartSearchCacheRegion.CreateChangeToken());
                using (var repository = RepositoryFactory())
                {
                    var sortInfos = GetSearchSortInfos(criteria);
                    var query = GetSearchQuery(repository, criteria, sortInfos);

                    retVal.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var cartIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        retVal.Results = (await CartService.GetByIdsAsync(cartIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }

                    return retVal;
                }
            });
        }

        protected virtual IQueryable<ShoppingCartEntity> GetQueryableShoppingCarts(ICartRepository repository)
        {
            return repository.ShoppingCarts;
        }

        protected virtual IList<SortInfo> GetSearchSortInfos(ShoppingCartSearchCriteria criteria)
        {
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

            return sortInfos;
        }

        protected virtual IQueryable<ShoppingCartEntity> GetSearchQuery(ICartRepository repository, ShoppingCartSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = GetQueryableShoppingCarts(repository);

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

            query.OrderBySortInfos(sortInfos);

            return query;
        }
    }
}
