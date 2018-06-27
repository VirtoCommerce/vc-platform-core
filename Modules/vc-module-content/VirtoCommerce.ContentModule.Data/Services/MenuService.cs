using System;
using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Data.Repositories;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class MenuService :  IMenuService
    {
        private readonly Func<IMenuRepository> _menuRepositoryFactory;

        public MenuService(Func<IMenuRepository> menuRepositoryFactory)
        {
            if (menuRepositoryFactory == null)
                throw new ArgumentNullException(nameof(menuRepositoryFactory));

            _menuRepositoryFactory = menuRepositoryFactory;
        }

        public IEnumerable<MenuLinkList> GetAllLinkLists()
        {
            return _menuRepositoryFactory().GetAllLinkLists();
        }

        public IEnumerable<MenuLinkList> GetListsByStoreId(string storeId)
        {
            return _menuRepositoryFactory().GetListsByStoreId(storeId);
        }

        public Models.MenuLinkList GetListById(string listId)
        {
            return _menuRepositoryFactory().GetListById(listId);
        }

        public void AddOrUpdate(Models.MenuLinkList list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            using (var repository = _menuRepositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                if (!list.IsTransient())
                {
                    var existList = repository.GetListById(list.Id);
                    if (existList != null)
                    {
                        changeTracker.Attach(existList);
                        list.Patch(existList);
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
                repository.UnitOfWork.Commit();
            }
        }

        public void DeleteList(string listId)
        {
            DeleteLists(new[] { listId });
        }

        public void DeleteLists(string[] listIds)
        {
            if (listIds == null)
                throw new ArgumentNullException(nameof(listIds));

            using (var repository = _menuRepositoryFactory())
            {
                foreach (var listId in listIds)
                {
                    var existList = repository.GetListById(listId);
                    if (existList != null)
                    {
                        repository.Remove(existList);
                    }
                }
                repository.UnitOfWork.Commit();
            }
        }


        public bool CheckList(string storeId, string name, string language, string id)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var lists = repository.GetListsByStoreId(storeId);

                var retVal = !lists.Any(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase)
                                    && (l.Language == language || (string.IsNullOrEmpty(l.Language) && string.IsNullOrEmpty(language)))
                                    && l.Id != id);

                return retVal;
            }
        }
    }
}
