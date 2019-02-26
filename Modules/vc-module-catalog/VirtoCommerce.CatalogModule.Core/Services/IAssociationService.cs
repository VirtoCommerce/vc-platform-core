using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IAssociationService
    {
        void LoadAssociations(IHasAssociations[] owners);

        void SaveChanges(IHasAssociations[] owners);
    }
}
