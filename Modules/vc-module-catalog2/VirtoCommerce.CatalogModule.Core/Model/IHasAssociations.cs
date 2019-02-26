using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public interface IHasAssociations : IEntity
    {
        IList<ProductAssociation> Associations { get; set; }

        IList<ProductAssociation> ReferencedAssociations { get; set; }
    }
}
