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
    public class PropertyDictionaryItemSearchService : IPropertyDictionaryItemSearchService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IPropertyDictionaryItemService _properyDictionaryItemService;

        public PropertyDictionaryItemSearchService(Func<ICatalogRepository> repositoryFactory, IPropertyDictionaryItemService properyDictionaryItemService)
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

                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await _properyDictionaryItemService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }

                return result;
            }
        }

        protected virtual IQueryable<PropertyDictionaryItemEntity> BuildQuery(ICatalogRepository repository, PropertyDictionaryItemSearchCriteria criteria)
        {
            var query = repository.PropertyDictionaryItems;
            if (!criteria.CatalogIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CatalogIds.Contains(x.Property.CatalogId));
            }
            if (!criteria.PropertyIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.PropertyIds.Contains(x.PropertyId));
            }
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Alias.Contains(criteria.Keyword));
            }

            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(PropertyDictionaryItemSearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(PropertyDictionaryItemEntity.SortOrder) },
                    new SortInfo { SortColumn = nameof(PropertyDictionaryItemEntity.Alias) }
               };
            }
            return sortInfos;
        }
    }
}
