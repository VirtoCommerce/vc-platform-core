using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Core.Model.Search
{
    public class VenderSearchResult : GenericSearchResult<Vendor>
    {
        public IList<Vendor> Vendors => Results;
    }
}
