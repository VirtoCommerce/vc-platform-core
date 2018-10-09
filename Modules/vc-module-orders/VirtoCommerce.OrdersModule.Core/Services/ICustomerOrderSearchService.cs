using System.Threading.Tasks;
using VirtoCommerce.OrdersModule.Core.Model;
using VirtoCommerce.OrdersModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrdersModule.Core.Services
{
    public interface ICustomerOrderSearchService
	{
        Task<GenericSearchResult<CustomerOrder>> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria criteria);
	}
}
