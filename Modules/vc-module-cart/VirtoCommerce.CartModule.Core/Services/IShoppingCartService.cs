using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;

namespace VirtoCommerce.CartModule.Core.Services
{
	public interface IShoppingCartService
	{
		Task<IEnumerable<ShoppingCart>> GetByIdsAsync(string[] cartIds, string responseGroup = null);
	    Task<ShoppingCart> GetByIdAsync(string cartId, string responseGroup = null);
        Task SaveChangesAsync(ShoppingCart[] carts);
		Task DeleteAsync(string[] cartIds);
	}
}
