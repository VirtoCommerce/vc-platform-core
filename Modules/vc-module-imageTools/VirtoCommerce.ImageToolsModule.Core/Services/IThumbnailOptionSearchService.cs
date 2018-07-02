using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailOptionSearchService
    {
        Task<GenericSearchResult<ThumbnailOption>> SearchAsync(ThumbnailOptionSearchCriteria criteria);
    }
}
