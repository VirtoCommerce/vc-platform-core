using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasAssets
    {
        IList<Asset> Assets { get; set; }
    }
}
