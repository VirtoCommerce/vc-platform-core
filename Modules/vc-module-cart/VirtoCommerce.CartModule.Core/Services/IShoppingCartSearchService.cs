using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model;
using VirtoCommerce.CartModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CartModule.Core.Services
{
	public interface IShoppingCartSearchService
	{
		Task<GenericSearchResult<ShoppingCart>> SearchCartAsync(ShoppingCartSearchCriteria criteria);
	}
}
