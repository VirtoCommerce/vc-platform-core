using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.MarketingModule.Core.Events;
using VirtoCommerce.MarketingModule.Core.Model.Promotions;
using VirtoCommerce.MarketingModule.Core.Services;
using VirtoCommerce.MarketingModule.Data.Caching;
using VirtoCommerce.MarketingModule.Data.Model;
using VirtoCommerce.MarketingModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.MarketingModule.Data.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly Func<IMarketingRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly IEventPublisher _eventPublisher;

        public PromotionService(Func<IMarketingRepository> repositoryFactory, IPlatformMemoryCache platformMemoryCache, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _eventPublisher = eventPublisher;
        }

        #region IMarketingService Members       

        public virtual async Task<Promotion[]> GetPromotionsByIdsAsync(string[] ids)
        {
            var cacheKey = CacheKey.With(GetType(), "GetPromotionsByIds", string.Join("-", ids));
            return await _platformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(PromotionCacheRegion.CreateChangeToken());
                using (var repository = _repositoryFactory())
                {
                    var promotionEntities = await repository.GetPromotionsByIdsAsync(ids);
                    return promotionEntities.Select(x => x.ToModel(AbstractTypeFactory<Promotion>.TryCreateInstance())).ToArray();
                }
            });
        }

        public virtual async Task SavePromotionsAsync(Promotion[] promotions)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<Promotion>>();
            using (var repository = _repositoryFactory())
            {
                var existEntities = await repository.GetPromotionsByIdsAsync(promotions.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var promotion in promotions)
                {
                    var sourceEntity = AbstractTypeFactory<PromotionEntity>.TryCreateInstance();
                    if (sourceEntity != null)
                    {
                        sourceEntity = sourceEntity.FromModel(promotion, pkMap);
                        var targetEntity = existEntities.FirstOrDefault(x => x.Id == promotion.Id);
                        if (targetEntity != null)
                        {
                            changedEntries.Add(new GenericChangedEntry<Promotion>(promotion, sourceEntity.ToModel(AbstractTypeFactory<Promotion>.TryCreateInstance()), EntryState.Modified));
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            changedEntries.Add(new GenericChangedEntry<Promotion>(promotion, EntryState.Added));
                            repository.Add(sourceEntity);
                        }
                    }
                }
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new PromotionChangedEvent(changedEntries));
            }

            PromotionCacheRegion.ExpireRegion();
        }

        public virtual async Task DeletePromotionsAsync(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                await repository.RemovePromotionsAsync(ids);
                await repository.UnitOfWork.CommitAsync();
                var changedEntries = new List<GenericChangedEntry<Promotion>>();
                foreach(var id in ids)
                {
                    var emptyPromotion = AbstractTypeFactory<Promotion>.TryCreateInstance();
                    emptyPromotion.Id = id;
                    changedEntries.Add(new GenericChangedEntry<Promotion>(emptyPromotion, EntryState.Deleted));
                }
                //Raise domain events after deletion
                await _eventPublisher.Publish(new PromotionChangedEvent(changedEntries));
            }

            PromotionCacheRegion.ExpireRegion();
        }

        #endregion
    }
}
