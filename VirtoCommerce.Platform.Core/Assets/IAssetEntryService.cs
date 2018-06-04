using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Assets
{
    public interface IAssetEntryService
    {
        Task<IEnumerable<AssetEntry>> GetByIdsAsync(IEnumerable<string> ids);
        void SaveChanges(IEnumerable<AssetEntry> items);
        void Delete(IEnumerable<string> ids);
    }
}
