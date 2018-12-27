using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.CartModule.Core.Events;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Services;
using VirtoCommerce.CartModule.Data.Caching;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.CartModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CartModule.Data.Services
{
    public class ShoppingCartServiceImpl : IShoppingCartService
    {
        public ShoppingCartServiceImpl(Func<ICartRepository> repositoryFactory, IDynamicPropertyService dynamicPropertyService,
                                      IShoppingCartTotalsCalculator totalsCalculator, IEventPublisher eventPublisher,
                                      IPlatformMemoryCache platformMemoryCache)
        {
            RepositoryFactory = repositoryFactory;
            EventPublisher = eventPublisher;
            DynamicPropertyService = dynamicPropertyService;
            TotalsCalculator = totalsCalculator;
            PlatformMemoryCache = platformMemoryCache;
        }

        protected Func<ICartRepository> RepositoryFactory { get; }
        protected IDynamicPropertyService DynamicPropertyService { get; }
        protected IShoppingCartTotalsCalculator TotalsCalculator { get; }
        protected IEventPublisher EventPublisher { get; }
        protected IPlatformMemoryCache PlatformMemoryCache { get; }

        #region IShoppingCartService Members

        public virtual async Task<IEnumerable<ShoppingCart>> GetByIdsAsync(string[] cartIds, string responseGroup = null)
        {
            var retVal = new List<ShoppingCart>();
            var cacheKey = CacheKey.With(GetType(), "GetByIdsAsync", string.Join("-", cartIds), responseGroup);
            return await PlatformMemoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                using (var repository = RepositoryFactory())
                {
                    //Disable DBContext change tracking for better performance 
                    repository.DisableChangesTracking();

                    var cartEntities = await repository.GetShoppingCartsByIdsAsync(cartIds, responseGroup);
                    foreach (var cartEntity in cartEntities)
                    {
                        var cart = cartEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance());
                        //Calculate totals only for full responseGroup
                        if (responseGroup == null)
                        {
                            TotalsCalculator.CalculateTotals(cart);
                        }
                        retVal.Add(cart);
                        cacheEntry.AddExpirationToken(CartCacheRegion.CreateChangeToken(cart));
                    }
                }

                await DynamicPropertyService.LoadDynamicPropertyValuesAsync(retVal.ToArray<IHasDynamicProperties>());

                return retVal;
            });
        }

        public virtual async Task<ShoppingCart> GetByIdAsync(string id, string responseGroup = null)
        {
            var carts = await GetByIdsAsync(new[] {id}, responseGroup);
            return carts.FirstOrDefault();
        }

        public virtual async Task SaveChangesAsync(ShoppingCart[] carts)
        {
            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<ShoppingCart>>();

            using (var repository = RepositoryFactory())
            {
                var dataExistCarts = await repository.GetShoppingCartsByIdsAsync(carts.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var cart in carts)
                {
                    //Calculate cart totals before save
                    TotalsCalculator.CalculateTotals(cart);

                    var originalEntity = dataExistCarts.FirstOrDefault(x => x.Id == cart.Id);
                    var modifiedEntity = AbstractTypeFactory<ShoppingCartEntity>.TryCreateInstance()
                                                                                .FromModel(cart, pkMap);
                    if (originalEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, originalEntity.ToModel(AbstractTypeFactory<ShoppingCart>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<ShoppingCart>(cart, EntryState.Added));
                    }
                }

                //Raise domain events
                await EventPublisher.Publish(new CartChangeEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await EventPublisher.Publish(new CartChangedEvent(changedEntries));
            }

            ClearCache(carts);
        }

        public virtual async Task DeleteAsync(string[] cartIds)
        {
            var carts = await GetByIdsAsync(cartIds);

            using (var repository = RepositoryFactory())
            {
                //Raise domain events before deletion
                var changedEntries = carts.Select(x => new GenericChangedEntry<ShoppingCart>(x, EntryState.Deleted));
                await EventPublisher.Publish(new CartChangeEvent(changedEntries));

                await repository.RemoveCartsAsync(cartIds);

                await repository.UnitOfWork.CommitAsync();
                //Raise domain events after deletion
                await EventPublisher.Publish(new CartChangedEvent(changedEntries));
            }

            ClearCache(carts);
        }

        protected virtual void ClearCache(IEnumerable<ShoppingCart> entities)
        {
            CartSearchCacheRegion.ExpireRegion();

            foreach (var entity in entities)
            {
                CartCacheRegion.ExpireInventory(entity);
            }
        }

        #endregion
    }
}
