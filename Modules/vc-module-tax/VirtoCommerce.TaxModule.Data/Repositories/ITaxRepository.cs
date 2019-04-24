using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.TaxModule.Data.Model;

namespace VirtoCommerce.TaxModule.Data.Repositories
{
    public interface ITaxRepository : IRepository
    {
        IQueryable<StoreTaxProviderEntity> StoreTaxProviders { get; }
        Task<StoreTaxProviderEntity[]> GetStoreTaxProviderByIdsAsync(string[] ids, string responseGroup = null);
    }
}
