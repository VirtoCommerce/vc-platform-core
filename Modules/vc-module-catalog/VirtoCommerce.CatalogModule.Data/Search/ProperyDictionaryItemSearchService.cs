using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Model;
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

               
                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] {
                        new SortInfo { SortColumn = "SortOrder", SortDirection = SortDirection.Ascending },
                        new SortInfo { SortColumn = "Alias", SortDirection = SortDirection.Ascending }
                    };
                }
                
                var query = BuildSearchQuery(repository, criteria, sortInfos);

                result.TotalCount = await query.CountAsync();

                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToListAsync();
                    var dictItems = await  _properyDictionaryItemService.GetByIdsAsync(ids.ToArray());
                    result.Results = dictItems.OrderBy(x => ids.IndexOf(x.Id)).ToList();
                }

                return result;
            }
        }

        protected virtual IQueryable<PropertyDictionaryItemEntity> BuildSearchQuery(ICatalogRepository repository, PropertyDictionaryItemSearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
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
    }
}
