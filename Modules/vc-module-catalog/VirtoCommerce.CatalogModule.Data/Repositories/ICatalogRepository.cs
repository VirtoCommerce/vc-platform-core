using System.Linq;
using VirtoCommerce.Domain.Catalog.Model.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using dataModel = VirtoCommerce.CatalogModule.Data.Model;
using moduleModel = VirtoCommerce.Domain.Catalog.Model;

namespace VirtoCommerce.CatalogModule.Data.Repositories
{
    public interface ICatalogRepository : IRepository
    {
        IQueryable<dataModel.CategoryEntity> Categories { get; }
        IQueryable<dataModel.CatalogEntity> Catalogs { get; }
        IQueryable<dataModel.ItemEntity> Items { get; }
        IQueryable<dataModel.PropertyEntity> Properties { get; }
        IQueryable<dataModel.ImageEntity> Images { get; }
        IQueryable<dataModel.AssetEntity> Assets { get; }
        IQueryable<dataModel.EditorialReviewEntity> EditorialReviews { get; }
        IQueryable<dataModel.PropertyValueEntity> PropertyValues { get; }
        IQueryable<dataModel.PropertyDictionaryValueEntity> PropertyDictionaryValues { get; }
        IQueryable<dataModel.PropertyDictionaryItemEntity> PropertyDictionaryItems { get; }
        IQueryable<dataModel.CategoryItemRelationEntity> CategoryItemRelations { get; }
        IQueryable<dataModel.AssociationEntity> Associations { get; }
        IQueryable<dataModel.CategoryRelationEntity> CategoryLinks { get; }

        string[] GetAllChildrenCategoriesIds(string[] categoryIds);
        dataModel.CatalogEntity[] GetCatalogsByIds(string[] catalogIds);
        dataModel.CategoryEntity[] GetCategoriesByIds(string[] categoryIds, moduleModel.CategoryResponseGroup respGroup);
        dataModel.ItemEntity[] GetItemByIds(string[] itemIds, moduleModel.ItemResponseGroup respGroup);
        dataModel.PropertyEntity[] GetAllCatalogProperties(string catalogId);
        dataModel.PropertyEntity[] GetPropertiesByIds(string[] propIds, bool loadDictValues = false);
        dataModel.PropertyDictionaryItemEntity[] GetPropertyDictionaryItemsByIds(string[] dictItemIds);


        GenericSearchResult<dataModel.AssociationEntity> SearchAssociations(ProductAssociationSearchCriteria criteria);

        void RemoveItems(string[] ids);
        void RemoveCategories(string[] ids);
        void RemoveCatalogs(string[] ids);
        void RemoveAllPropertyValues(string propertyId);
    }
}
