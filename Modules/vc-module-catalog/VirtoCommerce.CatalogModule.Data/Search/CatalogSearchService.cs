using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Search
{
    public class CatalogSearchService : ICatalogSearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly ICatalogService _catalogService;
        public CatalogSearchService(Func<ICatalogRepository> catalogRepositoryFactory, ICatalogService catalogService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _catalogService = catalogService;
        }

        public async Task<CatalogSearchResult> SearchCatalogsAsync(CatalogSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<CatalogSearchResult>.TryCreateInstance();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();
                var query = repository.Catalogs;
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(criteria.Keyword));
                }               
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }
                query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);
                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToListAsync();
                    var catalogs = await _catalogService.GetByIdsAsync(ids.ToArray(), criteria.ResponseGroup);
                    result.Results = catalogs.AsQueryable().OrderBySortInfos(sortInfos).ThenBy(x => x.Id).ToList();
                }
            }
            return result;
        }
    }
}
