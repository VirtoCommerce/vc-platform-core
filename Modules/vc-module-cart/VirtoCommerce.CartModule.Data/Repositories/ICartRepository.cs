using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Data.Repositories
{
    public interface ICartRepository : IRepository
    {
        IQueryable<ShoppingCartEntity> ShoppingCarts { get; }
        Task<ShoppingCartEntity[]> GetShoppingCartsByIdsAsync(string[] ids, string responseGroup = null);
        Task RemoveCartsAsync(string[] ids);
    }
}
