using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync();
        Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId);
        Task<MenuLinkList> GetListByIdAsync(string listId);
        Task AddOrUpdateAsync(MenuLinkList list);
        Task DeleteListAsync(string listId);
        Task DeleteListsAsync(string[] listIds);
        Task<bool> CheckListAsync(string storeId, string name, string language, string id);
    }
}
