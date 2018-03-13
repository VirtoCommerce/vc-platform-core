using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IAssociationService
    {
        void LoadAssociations(IEnumerable<IHasAssociations> owners);
        void SaveChanges(IEnumerable<IHasAssociations> owners);
    }
}
