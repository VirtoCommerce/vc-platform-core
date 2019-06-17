using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class MenuService : IMenuService
    {
        private readonly Func<IMenuRepository> _menuRepositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public MenuService(Func<IMenuRepository> menuRepositoryFactory, IEventPublisher eventPublisher)
        {
            _menuRepositoryFactory = menuRepositoryFactory;
            _eventPublisher = eventPublisher;
        }

        public async Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync()
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetAllLinkListsAsync();
                return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
            }
        }

        public async Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetListsByStoreIdAsync(storeId);
                return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
            }
        }

        public async Task<MenuLinkList> GetListByIdAsync(string listId)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetListByIdAsync(listId);
                return entities.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance());
            }
        }

        public async Task AddOrUpdateAsync(MenuLinkList list)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<MenuLinkList>>();
                var pkMap = new PrimaryKeyResolvingMap();

                var targetEntity = await repository.GetListByIdAsync(list.Id);
                var sourceEntity = AbstractTypeFactory<MenuLinkListEntity>.TryCreateInstance().FromModel(list, pkMap);

                if (targetEntity != null)
                {
                    changedEntries.Add(new GenericChangedEntry<MenuLinkList>(list, targetEntity.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()),
                        EntryState.Modified));
                    sourceEntity.Patch(targetEntity);
                }
                else
                {
                    repository.Add(sourceEntity);
                    changedEntries.Add(new GenericChangedEntry<MenuLinkList>(list, EntryState.Added));
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new MenuLinkListChangedEvent(changedEntries));
            }
        }

        public async Task DeleteListAsync(string listId)
        {
            await DeleteListsAsync(new[] { listId });
        }

        public async Task DeleteListsAsync(string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            using (var repository = _menuRepositoryFactory())
            {
                foreach (var listId in listIds)
                {
                    var existList = await repository.GetListByIdAsync(listId);
                    if (existList != null)
                    {
                        repository.Remove(existList);
                    }
                }
                await repository.UnitOfWork.CommitAsync();
            }
        }
        
        public async Task<bool> CheckListAsync(string storeId, string name, string language, string id)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var lists = await repository.GetListsByStoreIdAsync(storeId);

                var retVal = !lists.Any(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase)
                                    && (l.Language == language || (string.IsNullOrEmpty(l.Language) && string.IsNullOrEmpty(language)))
                                    && l.Id != id);

                return retVal;
            }
        }
    }
}
