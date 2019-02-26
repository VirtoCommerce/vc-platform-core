using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface ISkuGenerator
    {
        string GenerateSku(CatalogProduct product);
    }
}
