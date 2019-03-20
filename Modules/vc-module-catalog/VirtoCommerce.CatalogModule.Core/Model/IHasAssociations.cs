using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public interface IHasAssociations : IEntity
    {
        ICollection<ProductAssociation> Associations { get; set; }

        ICollection<ProductAssociation> ReferencedAssociations { get; set; }
    }
}
