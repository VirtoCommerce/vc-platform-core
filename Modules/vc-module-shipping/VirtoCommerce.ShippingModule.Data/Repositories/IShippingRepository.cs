using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.ShippingModule.Data.Model;

namespace VirtoCommerce.ShippingModule.Data.Repositories
{
    public interface IShippingRepository : IRepository
    {
        IQueryable<StoreShippingMethodEntity> StoreShippingMethods { get; }
        Task<StoreShippingMethodEntity[]> GetStoreShippingMethodsByIdsAsync(string[] ids, string responseGroup = null);
    }
}
