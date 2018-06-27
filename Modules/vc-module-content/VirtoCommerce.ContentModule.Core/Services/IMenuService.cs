using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuLinkList> GetAllLinkLists();
        IEnumerable<MenuLinkList> GetListsByStoreId(string storeId);
        MenuLinkList GetListById(string listId);
        void AddOrUpdate(MenuLinkList list);
        void DeleteList(string listId);
        void DeleteLists(string[] listIds);
        bool CheckList(string storeId, string name, string language, string id);
    }
}
