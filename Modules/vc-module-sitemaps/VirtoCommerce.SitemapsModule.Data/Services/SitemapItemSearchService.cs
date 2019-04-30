using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SitemapsModule.Core.Models.Search;
using VirtoCommerce.SitemapsModule.Core.Services;
using VirtoCommerce.SitemapsModule.Data.Models;
using VirtoCommerce.SitemapsModule.Data.Repositories;

namespace VirtoCommerce.SitemapsModule.Data.Services
{
    public class SitemapItemSearchService : ISitemapItemSearchService
    {
        private readonly Func<ISitemapRepository> _repositoryFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ISitemapItemService _sitemapItemService;

        public SitemapItemSearchService(Func<ISitemapRepository> repositoryFactory, ISitemapItemService sitemapItemService, IPlatformMemoryCache platformMemoryCache)
        {
            _repositoryFactory = repositoryFactory;
            _platformMemoryCache = platformMemoryCache;
            _sitemapItemService = sitemapItemService;
        }


        public virtual async Task<SitemapItemsSearchResult> SearchAsync(SitemapItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            var result = AbstractTypeFactory<SitemapItemsSearchResult>.TryCreateInstance();
            using (var repository = _repositoryFactory())
            {
                var query = repository.SitemapItems;
                if (!string.IsNullOrEmpty(criteria.SitemapId))
                {
                    query = query.Where(x => x.SitemapId == criteria.SitemapId);
                }

                if (criteria.ObjectTypes != null)
                {
                    query = query.Where(i =>
                        criteria.ObjectTypes.Contains(i.ObjectType, StringComparer.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(criteria.ObjectType))
                {
                    query = query.Where(i => i.ObjectType.EqualsInvariant(criteria.ObjectType));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                            new SortInfo
                            {
                                SortColumn = ReflectionUtility.GetPropertyName<SitemapItemEntity>(x => x.CreatedDate),
                                SortDirection = SortDirection.Descending
                            }
                        };
                }

                query = query.OrderBySortInfos(sortInfos);
                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var sitemapItemsIds = await query.Select(x => x.Id).Skip(criteria.Skip).Take(criteria.Take).ToArrayAsync();
                    result.Results = (await _sitemapItemService.GetByIdsAsync(sitemapItemsIds)).AsQueryable().OrderBySortInfos(sortInfos).ToArray();
                }
            }
            return result;
        }

    }
}
