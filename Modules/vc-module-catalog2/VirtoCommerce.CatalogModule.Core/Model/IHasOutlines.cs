using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasOutlines
    {
        IList<Outline> Outlines { get; set; }
    }
}
