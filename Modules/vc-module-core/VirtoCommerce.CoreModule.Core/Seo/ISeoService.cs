using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.CoreModule.Core.Seo
{
    /// <summary>
    /// Represent DAL for seo data
    /// </summary>
    public interface ISeoService
    {
        Task<IEnumerable<SeoInfo>> GetAllSeoDuplicatesAsync();
        Task<IEnumerable<SeoInfo>> GetSeoByKeywordAsync(string keyword);
        Task SaveSeoInfosAsync(SeoInfo[] seoinfos);
        Task LoadSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task SaveSeoForObjectsAsync(ISeoSupport[] seoSupportObjects);
        Task DeleteSeoForObjectAsync(ISeoSupport seoSupportObject);
    }
}
