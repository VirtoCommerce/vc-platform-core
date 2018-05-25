using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Platform.Core.Assets
{
    public interface IAssetEntrySearchService
    {
        AssetEntrySearchResult SearchAssetEntries(AssetEntrySearchCriteria criteria);
    }
}
