using System;
using System.Linq;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Core2.Model.Search;
using VirtoCommerce.CatalogModule.Core2.Services;
using VirtoCommerce.CatalogModule.Data2.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data2.Services
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

        public GenericSearchResult<Property> SearchProperties(PropertySearchCriteria criteria)
        {
            var result = new GenericSearchResult<Property>();

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var query = repository.Properties;
                if(!string.IsNullOrEmpty(criteria.CatalogId))
                {
                    query = query.Where(x => x.CatalogId == criteria.CatalogId);
                }
                if(!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Name.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Name" } };
                }
                query = query.OrderBySortInfos(sortInfos);
                result.TotalCount = query.Count();
                var ids = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.Id).ToList();

                var properties = _propertyService.GetByIds(ids);               
                result.Results = properties.OrderBy(x => ids.IndexOf(x.Id)).ToList();
            }
            return result;
        }

        public GenericSearchResult<PropertyDictionaryValue> SearchPropertyDictionaryValues(PropertyDictionaryValueSearchCriteria criteria)
        {
            var result = new GenericSearchResult<PropertyDictionaryValue>();

            using (var repository = _repositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var query = repository.PropertyDictionaryValues;
                if (!string.IsNullOrEmpty(criteria.PropertyId))
                {
                    query = query.Where(x => x.PropertyId == criteria.PropertyId);
                }
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    query = query.Where(x => x.Value.Contains(criteria.Keyword) || x.Alias.Contains(criteria.Keyword));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "Value" } };
                }
                query = query.OrderBySortInfos(sortInfos);
                result.TotalCount = query.Count();
                result.Results = query.Skip(criteria.Skip).Take(criteria.Take).Select(x => x.ToModel(AbstractTypeFactory<PropertyDictionaryValue>.TryCreateInstance())).ToList();
            }
            return result;
        }
    }
}
