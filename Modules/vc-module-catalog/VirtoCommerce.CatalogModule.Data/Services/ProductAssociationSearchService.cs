using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    public class ProductAssociationSearchService : IProductAssociationSearchService
    {
        private readonly Func<ICatalogRepository> _catalogRepositoryFactory;
        private readonly IItemService _itemService;
        public ProductAssociationSearchService(Func<ICatalogRepository> catalogRepositoryFactory, IItemService itemService)
        {
            _catalogRepositoryFactory = catalogRepositoryFactory;
            _itemService = itemService;
        }

        public async Task<GenericSearchResult<ProductAssociation>> SearchProductAssociationsAsync(ProductAssociationSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            if (criteria.ObjectIds.IsNullOrEmpty())
                return new GenericSearchResult<ProductAssociation>();

            using (var repository = _catalogRepositoryFactory())
            {
                //Optimize performance and CPU usage
                repository.DisableChangesTracking();

                var result = new GenericSearchResult<ProductAssociation>();

                var dbResult = await repository.SearchAssociations(criteria);

                result.TotalCount = dbResult.TotalCount;
                result.Results = dbResult.Results.Select(x => x.ToModel(AbstractTypeFactory<ProductAssociation>.TryCreateInstance())).ToList();
                return result;
            }
        }
    }
}
