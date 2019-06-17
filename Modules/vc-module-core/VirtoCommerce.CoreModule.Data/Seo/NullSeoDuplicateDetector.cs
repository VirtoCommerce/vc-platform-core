using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Data.Seo
{
    public class NullSeoDuplicateDetector : ISeoDuplicatesDetector
    {
        public Task<IEnumerable<SeoInfo>> DetectSeoDuplicatesAsync(TenantIdentity tenantIdentity)
        {
            return Task.FromResult(Enumerable.Empty<SeoInfo>());
        }
    }
}
