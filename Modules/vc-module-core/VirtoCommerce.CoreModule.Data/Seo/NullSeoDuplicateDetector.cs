using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.CoreModule.Data.Seo
{
    public class NullSeoDuplicateDetector : ISeoDuplicatesDetector
    {
        public Task<IEnumerable<SeoInfo>> DetectSeoDuplicatesAsync(string objectType, string objectId, IEnumerable<SeoInfo> allSeoDuplicates)
        {
            return Task.FromResult(Enumerable.Empty<SeoInfo>());
        }
    }
}
