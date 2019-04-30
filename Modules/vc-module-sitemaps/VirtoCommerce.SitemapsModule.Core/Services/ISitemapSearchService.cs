using System.Threading.Tasks;
using VirtoCommerce.SitemapsModule.Core.Models.Search;

namespace VirtoCommerce.SitemapsModule.Core.Services
{
    public interface ISitemapSearchService
    {
        Task<SitemapSearchResult> SearchAsync(SitemapSearchCriteria criteria);
    }
}
