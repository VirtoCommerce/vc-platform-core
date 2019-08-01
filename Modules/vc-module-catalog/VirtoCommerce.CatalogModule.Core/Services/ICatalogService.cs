using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogService
    {
        Task<Catalog[]> GetByIdsAsync(string[] catalogIds, string responseGroup = null);
        Task SaveChangesAsync(Catalog[] catalogs);
        Task DeleteAsync(string[] catalogIds);
    }
}
