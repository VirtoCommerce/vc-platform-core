using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Services
{
    public interface IAssociationService
    {
        Task LoadAssociationsAsync(IHasAssociations[] owners);

        Task SaveChangesAsync(IHasAssociations[] owners);
    }
}
