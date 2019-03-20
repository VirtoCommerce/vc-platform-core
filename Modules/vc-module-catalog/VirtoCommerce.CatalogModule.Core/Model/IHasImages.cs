using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasImages
    {
        ICollection<Image> Images { get; set; }
    }
}
