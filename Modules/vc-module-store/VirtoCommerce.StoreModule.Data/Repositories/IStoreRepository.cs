using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Data.Model;

namespace VirtoCommerce.StoreModule.Data.Repositories
{
    public interface IStoreRepository : IRepository
    {
        IQueryable<StoreEntity> Stores { get; }

        IQueryable<SeoInfoEntity> SeoInfos { get; }
        IQueryable<StoreDynamicPropertyObjectValueEntity> DynamicPropertyObjectValues { get; }
        Task<StoreEntity[]> GetStoresByIdsAsync(string[] ids, string responseGroup = null);

    }
}
