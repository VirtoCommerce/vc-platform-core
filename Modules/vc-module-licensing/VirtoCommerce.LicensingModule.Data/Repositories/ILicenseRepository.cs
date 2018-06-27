using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.LicensingModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.LicensingModule.Data.Repositories
{
    public interface ILicenseRepository : IRepository
    {
        IQueryable<LicenseEntity> Licenses { get; }

        Task<LicenseEntity[]> GetByIdsAsync(string[] ids);
        Task RemoveByIdsAsync(string[] ids);
    }
}
