using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Catalog>> GetCatalogsListAsync();
        Task<Catalog> GetByIdAsync(string catalogId);
        Task<Catalog> CreateAsync(Catalog catalog);
        Task UpdateAsync(Catalog[] catalogs);
        Task DeleteAsync(string[] catalogIds);
    }
}
