using System.Collections.Generic;
using VirtoCommerce.CatalogModule.Core2.Model;

namespace VirtoCommerce.CatalogModule.Core2.Services
{
    public interface IAssociationService
    {
        void LoadAssociations(IEnumerable<IHasAssociations> owners);
        void SaveChanges(IEnumerable<IHasAssociations> owners);
    }
}
