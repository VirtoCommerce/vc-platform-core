using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CoreModule.Core.Currency;
using VirtoCommerce.CoreModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.CoreModule.Data.Currency
{
    public class CurrencyService : ICurrencyService
    {
        private readonly Func<ICoreRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        private readonly IPlatformMemoryCache _platformMemoryCache;

        public CurrencyService(Func<ICoreRepository> repositoryFactory, IEventPublisher eventPublisher, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
            _platformMemoryCache = platformMemoryCache;
        }

        public async Task<IEnumerable<Core.Currency.Currency>> GetAllCurrenciesAsync()
        {
            var cacheKey = CacheKey.With(GetType(), "GetAllCurrenciesAsync");
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CurrencyCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var currencyEntities = await repository.Currencies.OrderByDescending(x => x.IsPrimary).ThenBy(x => x.Code).ToArrayAsync();
                    var result = currencyEntities.Select(x => x.ToModel(AbstractTypeFactory<Core.Currency.Currency>.TryCreateInstance())).ToList();

                    return result;
                }
            });
        }

        public async Task SaveChangesAsync(Core.Currency.Currency[] currencies)
        {
            if (currencies == null)
            {
                throw new ArgumentNullException(nameof(currencies));
            }

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                //Ensure that only one Primary currency
                if (currencies.Any(x => x.IsPrimary))
                {
                    var oldPrimaryCurrency = await repository.Currencies.FirstOrDefaultAsync(x => x.IsPrimary);

                    if (oldPrimaryCurrency != null)
                    {
                        oldPrimaryCurrency.IsPrimary = false;
                    }
                }

                foreach (var currency in currencies)
                {
                    var sourceEntry = AbstractTypeFactory<CurrencyEntity>.TryCreateInstance().FromModel(currency);
                    var targetEntry = await repository.Currencies.FirstOrDefaultAsync(x => x.Code == currency.Code);

                    if (targetEntry == null)
                    {
                        repository.Add(sourceEntry);
                    }
                    else
                    {
                        sourceEntry.Patch(targetEntry);
                    }
                }

                await repository.UnitOfWork.CommitAsync();

                CurrencyCacheRegion.ExpireRegion();
            }
        }

        public async Task DeleteCurrenciesAsync(string[] codes)
        {
            using (var repository = _repositoryFactory())
            {
                var currencyEntities = await repository.Currencies.Where(x => codes.Contains(x.Code)).ToArrayAsync();
                foreach (var currency in currencyEntities)
                {
                    if (currency.IsPrimary)
                    {
                        throw new ArgumentException("Unable to delete primary currency");
                    }

                    repository.Remove(currency);
                }

                await repository.UnitOfWork.CommitAsync();

                CurrencyCacheRegion.ExpireRegion();
            }
        }
    }
}
