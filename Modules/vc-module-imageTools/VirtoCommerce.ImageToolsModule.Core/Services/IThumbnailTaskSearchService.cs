using System.Threading.Tasks;
using VirtoCommerce.ImageToolsModule.Core.Models;

namespace VirtoCommerce.ImageToolsModule.Core.Services
{
    public interface IThumbnailTaskSearchService
    {
        Task<ThumbnailTaskSearchResult> SearchAsync(ThumbnailTaskSearchCriteria criteria);
    }
}
