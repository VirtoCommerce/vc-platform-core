using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IShoppingCartService _cartService;

        public ShoppingCartSearchServiceImpl(Func<ICartRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IShoppingCartService cartService)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _cartService = cartService;
        }


        public async Task<ShoppingCartSearchResult> SearchCartAsync(ShoppingCartSearchCriteria criteria)
        {
            var retVal = AbstractTypeFactory<ShoppingCartSearchResult>.TryCreateInstance();
            var cacheKey = CacheKey.With(GetType(), "SearchCartAsync", criteria.GetCacheKey());
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async cacheEntry =>
            {
                cacheEntry.AddExpirationToken(CartSearchCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(repository, criteria, sortInfos);

                    retVal.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var cartIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                        retVal.Results = (await _cartService.GetByIdsAsync(cartIds, criteria.ResponseGroup)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                    }

                    return retVal;
                }
            });
        }

        protected virtual IQueryable<ShoppingCartEntity> GetQuery(ICartRepository repository, ShoppingCartSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
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

        protected virtual IQueryable<ShoppingCartEntity> GetQueryableShoppingCarts(ICartRepository repository)
        {
            return repository.ShoppingCarts;
        }

        protected virtual IList<SortInfo> GetSortInfos(ShoppingCartSearchCriteria criteria)
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
    }
}
