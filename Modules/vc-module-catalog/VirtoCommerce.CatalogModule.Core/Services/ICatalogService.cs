using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ICatalogService
	{
        IEnumerable<Model.Catalog> GetAllCatalogs();
        IEnumerable<Model.Catalog> GetByIds(IEnumerable<string> catalogIds);
		void SaveChanges(IEnumerable<Model.Catalog> catalogs);
		void Delete(IEnumerable<string> catalogIds);
	}
}
