using System.Collections.Generic;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasOutlines
    {
        ICollection<Outline> Outlines { get; set; }
    }
}
