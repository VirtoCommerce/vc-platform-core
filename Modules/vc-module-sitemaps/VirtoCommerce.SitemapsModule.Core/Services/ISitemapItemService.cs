using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapItemService
    {
        Task<GenericSearchResult<SitemapItem>> SearchAsync(SitemapItemSearchCriteria criteria);

        Task SaveChangesAsync(SitemapItem[] items);

        Task RemoveAsync(string[] itemIds);
    }
}
