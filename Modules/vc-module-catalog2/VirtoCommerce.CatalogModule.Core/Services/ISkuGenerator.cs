using VirtoCommerce.CatalogModule.Core2.Model;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface ISkuGenerator
	{
		string GenerateSku(CatalogProduct product);
	}
}
