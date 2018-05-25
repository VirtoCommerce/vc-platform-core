using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Assets
{
    public class AssetEntrySearchResult
    {
        public int TotalCount { get; set; }
        public IList<AssetEntry> Results { get; set; }
    }
}
