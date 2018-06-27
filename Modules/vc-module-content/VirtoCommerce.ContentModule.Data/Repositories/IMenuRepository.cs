using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public interface IMenuRepository : IRepository
    {
        IEnumerable<MenuLinkList> GetAllLinkLists();
        IEnumerable<MenuLinkList> GetListsByStoreId(string storeId);
        MenuLinkList GetListById(string listId);
    }
}
