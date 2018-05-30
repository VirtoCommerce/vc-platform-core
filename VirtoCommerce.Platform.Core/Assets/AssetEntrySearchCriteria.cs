using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Assets
{
    public class AssetEntrySearchCriteria : SearchCriteriaBase
    {
        /// <summary>
        /// Phrase to search in Name and RelativeUrl
        /// </summary>
        public string SearchPhrase { get; set; }

        public TenantIdentity[] Tenants { get; set; }


        public string Group { get; set; }
    }
}
