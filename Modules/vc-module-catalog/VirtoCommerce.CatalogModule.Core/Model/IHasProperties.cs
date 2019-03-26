using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasProperties
    {
        IList<Property> Properties { get; set; }
    }
}
