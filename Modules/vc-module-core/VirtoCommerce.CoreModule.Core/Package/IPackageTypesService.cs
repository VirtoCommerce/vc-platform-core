using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.CoreModule.Core.Package
{
    public interface IPackageTypesService
    {
        Task<IEnumerable<PackageType>> GetAllPackageTypesAsync();
        Task SaveChangesAsync(PackageType[] packageTypes);
        Task DeletePackageTypesAsync(string[] ids);
    }
}
