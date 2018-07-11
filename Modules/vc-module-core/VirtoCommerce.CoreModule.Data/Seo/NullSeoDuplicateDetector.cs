using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.CoreModule.Data.Seo
{
    public class NullSeoDuplicateDetector : ISeoDuplicatesDetector
    {
        public IEnumerable<SeoInfo> DetectSeoDuplicates(string objectType, string objectId, IEnumerable<SeoInfo> allSeoDuplicates)
        {
            return Enumerable.Empty<SeoInfo>();
        }
    }
}
