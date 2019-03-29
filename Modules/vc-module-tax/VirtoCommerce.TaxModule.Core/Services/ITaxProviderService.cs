using System.Threading.Tasks;
using VirtoCommerce.TaxModule.Core.Model;

namespace VirtoCommerce.TaxModule.Core.Services
{
    public interface ITaxProviderService
    {
        Task<TaxProvider[]> GetByIdsAsync(string[] ids, string responseGroup);
        Task<TaxProvider> GetByIdAsync(string id, string responseGroup);
        Task SaveChangesAsync(TaxProvider[] taxProviders);
        Task DeleteAsync(string[] ids);
    }
}
