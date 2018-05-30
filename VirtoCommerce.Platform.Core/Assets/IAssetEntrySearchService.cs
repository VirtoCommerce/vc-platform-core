using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Assets
{
    public interface IAssetEntrySearchService
    {
        GenericSearchResult<AssetEntry> SearchAssetEntries(AssetEntrySearchCriteria criteria);
    }
}
