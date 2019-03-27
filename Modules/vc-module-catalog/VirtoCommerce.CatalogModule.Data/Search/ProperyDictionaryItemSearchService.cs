using System;
using System.Collections.Generic;
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
    public class ProperyDictionaryItemSearchService : IProperyDictionaryItemSearchService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IProperyDictionaryItemService _properyDictionaryItemService;

        public ProperyDictionaryItemSearchService(Func<ICatalogRepository> repositoryFactory, IProperyDictionaryItemService properyDictionaryItemService)
        {
            _repositoryFactory = repositoryFactory;
            _properyDictionaryItemService = properyDictionaryItemService;
        }

        public async Task<PropertyDictionaryItemSearchResult> SearchAsync(PropertyDictionaryItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var result = AbstractTypeFactory<PropertyDictionaryItemSearchResult>.TryCreateInstance();

                var sortInfos = GetSearchSortInfo(criteria);
                var query = GetSearchQuery(criteria, repository, sortInfos);

                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArrayAsync();
                    result.Results = (await _properyDictionaryItemService.GetByIdsAsync(ids)).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                }

                return result;
            }
        }

        private IQueryable<Model.PropertyDictionaryItemEntity> GetSearchQuery(PropertyDictionaryItemSearchCriteria criteria, ICatalogRepository repository, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.PropertyDictionaryItems;

            if (!criteria.PropertyIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.PropertyIds.Contains(x.PropertyId));
            }

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Alias.Contains(criteria.Keyword));
            }

            query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);
            return query;
        }

        private IList<SortInfo> GetSearchSortInfo(PropertyDictionaryItemSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[] {
                        new SortInfo { SortColumn = "SortOrder", SortDirection = SortDirection.Ascending },
                        new SortInfo { SortColumn = "Alias", SortDirection = SortDirection.Ascending }
                    };
            }

            return sortInfos;
        }
    }
}
