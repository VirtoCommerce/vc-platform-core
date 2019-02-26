using System.Linq;
using VirtoCommerce.CatalogModule.Core2.Model;
using VirtoCommerce.CatalogModule.Data2.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Data2.Repositories
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
        IQueryable<CategoryItemRelationEntity> CategoryItemRelations { get; }
        IQueryable<AssociationEntity> Associations { get; }
        IQueryable<CategoryRelationEntity> CategoryLinks { get; }

        string[] GetAllChildrenCategoriesIds(string[] categoryIds);
        CatalogEntity[] GetCatalogsByIds(string[] catalogIds);
        CategoryEntity[] GetCategoriesByIds(string[] categoryIds, string respGroup = null);
        ItemEntity[] GetItemByIds(string[] itemIds, ItemResponseGroup respGroup);
        PropertyEntity[] GetAllCatalogProperties(string catalogId);
        PropertyEntity[] GetPropertiesByIds(string[] propIds);
 
        void RemoveItems(string[] ids);
        void RemoveCategories(string[] ids);
        void RemoveCatalogs(string[] ids);
        void RemoveAllPropertyValues(string propertyId);
    }
}
