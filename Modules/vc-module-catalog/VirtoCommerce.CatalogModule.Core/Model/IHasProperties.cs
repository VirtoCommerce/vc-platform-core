using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasProperties
    {
        ICollection<Property> Properties { get; set; }
    }
}
