using System.Threading.Tasks;
using VirtoCommerce.OrderModule.Core.Model;
using VirtoCommerce.OrderModule.Core.Model.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.OrderModule.Core.Services
{
    public interface ICustomerOrderSearchService
	{
        Task<GenericSearchResult<CustomerOrder>> SearchCustomerOrdersAsync(CustomerOrderSearchCriteria criteria);
	}
}
