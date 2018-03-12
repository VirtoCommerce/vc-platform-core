using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasOutlines
    {
        IList<Outline> Outlines { get; set; }
    }
}
