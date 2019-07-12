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

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                            new SortInfo
                            {
                                SortColumn = "Name"
                            }
                        };
                }
                var query = BuildSearchQuery(repository, criteria, sortInfos);
                result.TotalCount = await query.CountAsync();
                if (criteria.Take > 0)
                {
                    var ids = await query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToListAsync();
                    var properties = await _propertyService.GetByIdsAsync(ids);
                    result.Results = properties.OrderBy(x => ids.IndexOf(x.Id)).ToList();
                }
            }
            return result;
        }


        protected virtual IQueryable<PropertyEntity> BuildSearchQuery(ICatalogRepository repository, PropertySearchCriteria criteria, IEnumerable<SortInfo> sortInfos)
        {
            var query = repository.Properties;
            if (!string.IsNullOrEmpty(criteria.CatalogId))
            {
                query = query.Where(x => x.CatalogId == criteria.CatalogId);
            }
            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                query = query.Where(x => x.Name.Contains(criteria.Keyword));
            }
            if (!criteria.PropertyNames.IsNullOrEmpty())
            {
                query = query.Where(x => criteria.PropertyNames.Contains(x.Name));
            }

            query = query.OrderBySortInfos(sortInfos).ThenBy(x => x.Id);
            return query;
        }
    }
}
