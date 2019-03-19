using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public interface IMenuRepository : IRepository
    {
        Task<IEnumerable<MenuLinkListEntity>> GetAllLinkListsAsync();
        Task<IEnumerable<MenuLinkListEntity>> GetListsByStoreIdAsync(string storeId);
        Task<MenuLinkListEntity> GetListByIdAsync(string listId);
    }
}
