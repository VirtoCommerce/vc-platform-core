using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Model;

namespace VirtoCommerce.CoreModule.Core.Services
{
    public interface IPackageTypesService
    {
        Task<IEnumerable<PackageType>> GetAllPackageTypesAsync();
        Task UpsertPackageTypesAsync(PackageType[] packageTypes);
        Task DeletePackageTypesAsync(string[] ids);
    }
}
