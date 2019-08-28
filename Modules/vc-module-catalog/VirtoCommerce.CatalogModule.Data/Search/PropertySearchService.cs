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
    public class PropertySearchService : IPropertySearchService
    {
        private readonly Func<ICatalogRepository> _repositoryFactory;
        private readonly IPropertyService _propertyService;
        public PropertySearchService(Func<ICatalogRepository> repositoryFactory, IPropertyService propertyService)
        {
            _repositoryFactory = repositoryFactory;
            _propertyService = propertyService;
        }

        public async Task<PropertySearchResult> SearchPropertiesAsync(PropertySearchCriteria criteria)
        {
            var result = AbstractTypeFactory<PropertySearchResult>.TryCreateInstance();

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var sortInfos = BuildSortExpression(criteria);
                var query = BuildQuery(repository, criteria);

                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id)
                                        .Select(x => x.Id)
                                        .Skip(criteria.Skip).Take(criteria.Take)
                                        .ToArrayAsync();

                    result.Results = (await _propertyService.GetByIdsAsync(ids)).OrderBy(x => Array.IndexOf(ids, x.Id)).ToList();
                }
            }
            return result;
        }


        protected virtual IQueryable<PropertyEntity> BuildQuery(ICatalogRepository repository, PropertySearchCriteria criteria)
        {
            var query = repository.Properties;
            if (!criteria.CatalogIds.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.CatalogIds.Contains(x.CatalogId));
            }
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }
            if (!criteria.PropertyNames.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.PropertyNames.Contains(x.Name));
            }
            return query;
        }

        protected virtual IList<SortInfo> BuildSortExpression(PropertySearchCriteria criteria)
        {
            var sortInfos = criteria.SortInfos;
            if (sortInfos.IsNullOrEmpty())
            {
                sortInfos = new[]
                {
                    new SortInfo { SortColumn = nameof(PropertyEntity.Name) }
                };
            }
            return sortInfos;
        }
    }
}
