using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasAssets
    {
        ICollection<Asset> Assets { get; set; }
    }
}
