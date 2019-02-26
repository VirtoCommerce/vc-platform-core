using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasProperties
    {
        IList<Property> Properties { get; set; }
    }
}
