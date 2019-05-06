using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.SitemapsModule.Core.Models;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapItemService
    {
        Task<IEnumerable<SitemapItem>> GetByIdsAsync(string[] itemIds, string responseGroup = null);
        Task SaveChangesAsync(SitemapItem[] items);

        Task RemoveAsync(string[] itemIds);
    }
}
