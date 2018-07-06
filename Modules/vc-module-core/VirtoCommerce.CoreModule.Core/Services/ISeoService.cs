using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Model;

namespace VirtoCommerce.CoreModule.Core.Services
{
    public interface ISeoService
    {
        Task<IEnumerable<SeoInfo>> GetAllSeoDuplicatesAsync();
        Task<IEnumerable<SeoInfo>> GetSeoByKeywordAsync(string keyword);
        Task UpsertSeoInfosAsync(SeoInfo[] seoinfos);
        Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task UpsertSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject);
    }
}
