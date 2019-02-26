using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogService
    {
        IEnumerable<Catalog> GetCatalogsList();
        Catalog GetById(string catalogId);
        Catalog Create(Catalog catalog);
        void Update(Catalog[] catalogs);
        void Delete(string[] catalogIds);
    }
}
