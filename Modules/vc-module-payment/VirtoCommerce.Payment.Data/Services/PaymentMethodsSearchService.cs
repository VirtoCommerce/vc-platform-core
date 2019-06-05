using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.PaymentModule.Core.Model;
using VirtoCommerce.PaymentModule.Core.Model.Search;
using VirtoCommerce.PaymentModule.Core.Services;
using VirtoCommerce.PaymentModule.Data.Caching;
using VirtoCommerce.PaymentModule.Data.Model;
using VirtoCommerce.PaymentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.PaymentModule.Data.Services
{
    public class PaymentMethodsSearchService : IPaymentMethodsSearchService
    {
        private readonly Func<IPaymentRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _memCache;
        private readonly IPaymentMethodsService _paymentMethodsService;
        private readonly ISettingsManager _settingsManager;


        public PaymentMethodsSearchService(
            Func<IPaymentRepository> repositoryFactory,
            IPlatformMemoryCache memCache,
            IPaymentMethodsService paymentMethodsService,
            ISettingsManager settingsManager)
        {
            _repositoryFactory = repositoryFactory;
            _memCache = memCache;
            _paymentMethodsService = paymentMethodsService;
            _settingsManager = settingsManager;
        }

        public async Task<PaymentMethodsSearchResult> SearchPaymentMethodsAsync(PaymentMethodsSearchCriteria criteria)
        {
            var cacheKey = CacheKey.With(GetType(), "SearchPaymentMethodsAsync", criteria.GetCacheKey());
            return await _memCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PaymentCacheRegion.CreateChangeToken());
                var result = AbstractTypeFactory<PaymentMethodsSearchResult>.TryCreateInstance();

                var tmpSkip = 0;
                var tmpTake = 0;

                var sortInfos = GetSortInfos(criteria);

                using (var repository = _repositoryFactory())
                {
                    repository.DisableChangesTracking();


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
                //Need to concatenate  persistent methods with registered types and still not persisted
                tmpSkip = Math.Min(result.TotalCount, criteria.Skip);
                tmpTake = Math.Min(criteria.Take, Math.Max(0, result.TotalCount - criteria.Skip));
                criteria.Skip = criteria.Skip - tmpSkip;
                criteria.Take = criteria.Take - tmpTake;
                if (criteria.Take > 0 && !criteria.WithoutTransient)
                {
                    var transientMethodsQuery = AbstractTypeFactory<PaymentMethod>.AllTypeInfos.Select(x => AbstractTypeFactory<PaymentMethod>.TryCreateInstance(x.Type.Name))
                                                                                  .OfType<PaymentMethod>().AsQueryable();
                    if (!string.IsNullOrEmpty(criteria.Keyword))
                    {
                        transientMethodsQuery = transientMethodsQuery.Where(x => x.Code.Contains(criteria.Keyword));
                    }
                    var allPersistentTypes = result.Results.Select(x => x.GetType()).Distinct();
                    transientMethodsQuery = transientMethodsQuery.Where(x => !allPersistentTypes.Contains(x.GetType()));

                    result.TotalCount += transientMethodsQuery.Count();
                    var transientProviders = transientMethodsQuery.Skip(criteria.Skip).Take(criteria.Take).ToList();

                    foreach (var transientProvider in transientProviders)
                    {
                        await _settingsManager.DeepLoadSettingsAsync(transientProvider);
                    }

                    result.Results = result.Results.Concat(transientProviders).AsQueryable().OrderBySortInfos(sortInfos).ToList();
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

            if (!criteria.Codes.IsNullOrEmpty())
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
