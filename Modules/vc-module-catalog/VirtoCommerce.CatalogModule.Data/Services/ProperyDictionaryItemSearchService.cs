using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
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

        public async Task<GenericSearchResult<PropertyDictionaryItem>> SearchAsync(PropertyDictionaryItemSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var result = new GenericSearchResult<PropertyDictionaryItem>();

                var query = repository.PropertyDictionaryItems;
                if (!criteria.PropertyIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.PropertyIds.Contains(x.PropertyId));
                }
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Alias.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] {
                        new SortInfo { SortColumn = "SortOrder", SortDirection = SortDirection.Ascending },
                        new SortInfo { SortColumn = "Alias", SortDirection = SortDirection.Ascending }
                    };
                }

                query = query.OrderBySortInfos(sortInfos);

                result.TotalCount = await query.CountAsync();

                var ids = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToArray();
                result.Results = (await _properyDictionaryItemService.GetByIdsAsync(ids)).AsQueryable().OrderBySortInfos(sortInfos).ToList();
                return result;
            }
        }
    }
}
