namespace VirtoCommerce.Domain.Catalog.Services
{
    public interface ICatalogService
	{
        Model.Catalog[] GetAllCatalogs();
        Model.Catalog[] GetByIds(string[] catalogIds);
		void SaveChanges(Model.Catalog[] catalogs);
		void Delete(string[] catalogIds);
	}
}
