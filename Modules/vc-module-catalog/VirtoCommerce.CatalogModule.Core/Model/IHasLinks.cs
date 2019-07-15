using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasLinks : IEntity, IHasCatalogId
    {
        IList<CategoryLink> Links { get; set; }
    }
}
