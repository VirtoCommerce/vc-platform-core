using System.Threading.Tasks;
using VirtoCommerce.CartModule.Core.Model.Search;

namespace VirtoCommerce.CartModule.Core.Services
{
    public interface IShoppingCartSearchService
    {
        Task<ShoppingCartSearchResult> SearchCartAsync(ShoppingCartSearchCriteria criteria);
    }
}
