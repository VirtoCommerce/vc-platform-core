using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.InventoryModule.Core.Events;
using VirtoCommerce.InventoryModule.Core.Model;
using VirtoCommerce.InventoryModule.Core.Services;
using VirtoCommerce.InventoryModule.Data.Model;
using VirtoCommerce.InventoryModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.InventoryModule.Data.Services
{
    public class FulfillmentCenterService : IFulfillmentCenterService
    {
        private readonly Func<IInventoryRepository> _repositoryFactory;
        private readonly IEventPublisher _eventPublisher;
        public FulfillmentCenterService(Func<IInventoryRepository> repositoryFactory, IEventPublisher eventPublisher)
        {
            _repositoryFactory = repositoryFactory;
            _eventPublisher = eventPublisher;
        }

        #region IFulfillmentCenterService members
        public async Task<IEnumerable<FulfillmentCenter>> GetByIdsAsync(IEnumerable<string> ids, string responseGroup = null)
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }
            IEnumerable<FulfillmentCenter> result = null;
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var fulfillmentCenters = await repository.GetFulfillmentCenters(ids);
                result = fulfillmentCenters.Select(x => x.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance())).ToArray();
            }
            return result;
        }

        public async Task SaveChangesAsync(IEnumerable<FulfillmentCenter> fulfillmentCenters)
        {
            if (fulfillmentCenters == null)
            {
                throw new ArgumentNullException(nameof(fulfillmentCenters));
            }

            var pkMap = new PrimaryKeyResolvingMap();
            var changedEntries = new List<GenericChangedEntry<FulfillmentCenter>>();
            using (var repository = _repositoryFactory())
            {
                var existEntities = await repository.GetFulfillmentCenters(fulfillmentCenters.Where(x => !x.IsTransient()).Select(x => x.Id).ToArray());
                foreach (var changedCenter in fulfillmentCenters)
                {
                    var existEntity = existEntities.FirstOrDefault(x => x.Id == changedCenter.Id);
                    var modifiedEntity = AbstractTypeFactory<FulfillmentCenterEntity>.TryCreateInstance().FromModel(changedCenter, pkMap);
                    if (existEntity != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<FulfillmentCenter>(changedCenter, existEntity.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance()), EntryState.Modified));
                        modifiedEntity.Patch(existEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                        changedEntries.Add(new GenericChangedEntry<FulfillmentCenter>(changedCenter, EntryState.Added));
                    }
                }
                //Raise domain events
                await _eventPublisher.Publish(new FulfillmentCenterChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new FulfillmentCenterChangedEvent(changedEntries));
            }
        }

        public async Task DeleteAsync(IEnumerable<string> ids)
        {
            using (var repository = _repositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<FulfillmentCenter>>();
                var dbCenters = await repository.GetFulfillmentCenters(ids);
                foreach (var dbCenter in dbCenters)
                {
                    repository.Remove(dbCenter);
                    changedEntries.Add(new GenericChangedEntry<FulfillmentCenter>(dbCenter.ToModel(AbstractTypeFactory<FulfillmentCenter>.TryCreateInstance()), EntryState.Deleted));
                }

                await _eventPublisher.Publish(new FulfillmentCenterChangingEvent(changedEntries));
                await repository.UnitOfWork.CommitAsync();
                await _eventPublisher.Publish(new FulfillmentCenterChangedEvent(changedEntries));
            }
        }
        #endregion
    }
}
