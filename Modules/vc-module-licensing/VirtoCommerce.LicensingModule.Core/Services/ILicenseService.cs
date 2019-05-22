using System.Threading.Tasks;
using VirtoCommerce.LicensingModule.Core.Model;

namespace VirtoCommerce.LicensingModule.Core.Services
{
    public interface ILicenseService
    {
        Task<LicenseSearchResult> SearchAsync(LicenseSearchCriteria criteria);
        Task<License[]> GetByIdsAsync(string[] ids);
        Task SaveChangesAsync(License[] licenses);
        Task DeleteAsync(string[] ids);
        Task<string> GetSignedLicenseAsync(string code, string clientIpAddress, bool isActivated);
    }
}
