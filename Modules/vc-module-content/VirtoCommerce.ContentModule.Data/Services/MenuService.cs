using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Converters;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class MenuService :  IMenuService
    {
        private readonly Func<IMenuRepository> _menuRepositoryFactory;

        public MenuService(Func<IMenuRepository> menuRepositoryFactory)
        {
            _menuRepositoryFactory = menuRepositoryFactory;
        }

        public async Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync()
        {
            var entities =  await _menuRepositoryFactory().GetAllLinkListsAsync();
            return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
        }

        public async Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId)
        {
            var entities = await _menuRepositoryFactory().GetListsByStoreIdAsync(storeId);
            return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
        }

        public async Task<MenuLinkList> GetListByIdAsync(string listId)
        {
            var entities = await _menuRepositoryFactory().GetListByIdAsync(listId);
            return entities.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance());
        }

        public async Task AddOrUpdateAsync(MenuLinkList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            using (var repository = _menuRepositoryFactory())
            {
                if (!list.IsTransient())
                {
                    var existList = await repository.GetListByIdAsync(list.Id);

                    if (existList != null)
                    {
                        list.Patch(existList.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
                    }
                    else
                    {
                        repository.Add(list);
                    }
                }
                else
                {
                    repository.Add(list);
                }

                await repository.UnitOfWork.CommitAsync();
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
