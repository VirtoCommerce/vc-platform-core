using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data.Repositories
{
    public interface ICatalogRepository : IRepository
    {
        IQueryable<CategoryEntity> Categories { get; }
        IQueryable<CatalogEntity> Catalogs { get; }
        IQueryable<ItemEntity> Items { get; }
        IQueryable<PropertyEntity> Properties { get; }
        IQueryable<ImageEntity> Images { get; }
        IQueryable<AssetEntity> Assets { get; }
        IQueryable<EditorialReviewEntity> EditorialReviews { get; }
        IQueryable<PropertyValueEntity> PropertyValues { get; }
        IQueryable<PropertyDictionaryValueEntity> PropertyDictionaryValues { get; }
        IQueryable<PropertyDictionaryItemEntity> PropertyDictionaryItems { get; }
        IQueryable<CategoryItemRelationEntity> CategoryItemRelations { get; }
        IQueryable<AssociationEntity> Associations { get; }
        IQueryable<CategoryRelationEntity> CategoryLinks { get; }
        IQueryable<SeoInfoEntity> SeoInfos { get; }

        Task<string[]> GetAllChildrenCategoriesIdsAsync(string[] categoryIds);
        Task<CatalogEntity[]> GetCatalogsByIdsAsync(string[] catalogIds);
        Task<CategoryEntity[]> GetCategoriesByIdsAsync(string[] categoriesIds, string responseGroup);
        Task<ItemEntity[]> GetItemByIdsAsync(string[] itemIds, string responseGroup = null);
        Task<PropertyEntity[]> GetAllCatalogPropertiesAsync(string catalogId);
        Task<PropertyEntity[]> GetPropertiesByIdsAsync(string[] propIds, bool loadDictValues = false);
        Task<PropertyDictionaryItemEntity[]> GetPropertyDictionaryItemsByIdsAsync(string[] dictItemIds);

        Task<GenericSearchResult<AssociationEntity>> SearchAssociations(ProductAssociationSearchCriteria criteria);

        Task RemoveItemsAsync(string[] itemIds);
        Task RemoveCategoriesAsync(string[] ids);
        Task RemoveCatalogsAsync(string[] ids);
        Task RemoveAllPropertyValuesAsync(string propertyId);
    }
}
