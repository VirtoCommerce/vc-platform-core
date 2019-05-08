using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.PaymentModule.Core.Models.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Data.Caching;
using VirtoCommerce.PaymentModule.Data.Model;
using VirtoCommerce.PaymentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.PaymentModule.Data.Services
{
    public class PaymentMethodsSearchService : IPaymentMethodsSearchService
    {
        private readonly Func<IPaymentRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memCache;
        private readonly IPaymentMethodsService _paymentMethodsService;

        public PaymentMethodsSearchService(
            Func<IPaymentRepository> repositoryFactory,
            IPlatformMemoryCache memCache,
            IPaymentMethodsService paymentMethodsService)
        {
            _repositoryFactory = repositoryFactory;
            _memCache = memCache;
            _paymentMethodsService = paymentMethodsService;
        }

        public async Task<PaymentMethodsSearchResult> SearchPaymentMethodsAsync(PaymentMethodsSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchPaymentMethodsAsync", criteria.GetCacheKey());
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PaymentCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<PaymentMethodsSearchResult>.TryCreateInstance();
                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();

                    var sortInfos = GetSortInfos(criteria);
                    var query = GetQuery(repository, criteria, sortInfos);

                    result.TotalCount = await query.CountAsync();
                    if (criteria.Take > 0)
                    {
                        var paymentMethodsIds = await query.Select(x => x.Id)
                                                           .Skip(criteria.Skip)
                                                           .Take(criteria.Take)
                                                           .ToArrayAsync();

                        result.Results = (await _paymentMethodsService.GetByIdsAsync(paymentMethodsIds, criteria.ResponseGroup))
                                                                      .AsQueryable()
                                                                      .OrderBySortInfos(sortInfos)
                                                                      .ToArray();
                    }
                }

                return result;
            });
        }

        protected virtual IQueryable<StorePaymentMethodEntity> GetQuery(
            IPaymentRepository repository,
            PaymentMethodsSearchCriteria criteria,
            IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.StorePaymentMethods;

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Code.Contains(criteria.Keyword) || x.Id.Contains(criteria.Keyword));
            }

            if (!criteria.StoreId.IsNullOrEmpty())
            {
                query = query.Where(x => x.StoreId == criteria.StoreId);
            }

            if (criteria.Codes.Any())
            {
                query = query.Where(x => criteria.Codes.Contains(x.Code));
            }

            if (criteria.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == criteria.IsActive.Value);
            }

            query = query.OrderBySortInfos(sortInfos);
            return query;
        }

        protected virtual IList<SortInfo> GetSortInfos(PaymentMethodsSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo{ SortColumn = "Name" }
                };
            }

            return sortInfos;
        }
    }
}
