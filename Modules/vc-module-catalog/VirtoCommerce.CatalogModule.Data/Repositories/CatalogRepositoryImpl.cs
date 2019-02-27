using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Repositories
{
    public class CatalogRepositoryImpl : DbContextRepositoryBase<CatalogDbContext>, ICatalogRepository
    {
        private readonly CatalogDbContext _dbContext;
        public CatalogRepositoryImpl(CatalogDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        #region ICatalogRepository Members

        public IQueryable<CategoryEntity> Categories => DbContext.Set<CategoryEntity>();
        public IQueryable<CatalogEntity> Catalogs => DbContext.Set<CatalogEntity>();
        public IQueryable<PropertyValueEntity> PropertyValues => DbContext.Set<PropertyValueEntity>();
        public IQueryable<ImageEntity> Images => DbContext.Set<ImageEntity>();
        public IQueryable<AssetEntity> Assets => DbContext.Set<AssetEntity>();
        public IQueryable<ItemEntity> Items => DbContext.Set<ItemEntity>();
        public IQueryable<EditorialReviewEntity> EditorialReviews => DbContext.Set<EditorialReviewEntity>();
        public IQueryable<PropertyEntity> Properties => DbContext.Set<PropertyEntity>();
        public IQueryable<PropertyDictionaryItemEntity> PropertyDictionaryItems => DbContext.Set<PropertyDictionaryItemEntity>();
        public IQueryable<PropertyDictionaryValueEntity> PropertyDictionaryValues => DbContext.Set<PropertyDictionaryValueEntity>();
        public IQueryable<PropertyDisplayNameEntity> PropertyDisplayNames => DbContext.Set<PropertyDisplayNameEntity>();
        public IQueryable<PropertyAttributeEntity> PropertyAttributes => DbContext.Set<PropertyAttributeEntity>();
        public IQueryable<CategoryItemRelationEntity> CategoryItemRelations => DbContext.Set<CategoryItemRelationEntity>();
        public IQueryable<AssociationEntity> Associations => DbContext.Set<AssociationEntity>();
        public IQueryable<CategoryRelationEntity> CategoryLinks => DbContext.Set<CategoryRelationEntity>();
        public IQueryable<PropertyValidationRuleEntity> PropertyValidationRules => DbContext.Set<PropertyValidationRuleEntity>();

        public async Task<CatalogEntity[]> GetCatalogsByIdsAsync(string[] catalogIds)
        {
            var retVal = await Catalogs.Include(x => x.CatalogLanguages)
                                 .Include(x => x.IncommingLinks)
                                 .Where(x => catalogIds.Contains(x.Id))
                                 .ToArrayAsync();

            var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryValueEntities).Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null).ToArrayAsync();
            var catalogPropertiesIds = await Properties.Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null)
                                                 .Select(x => x.Id)
                                                 .ToArrayAsync();
            var catalogProperties = GetPropertiesByIdsAsync(catalogPropertiesIds);

            return retVal;
        }

        public async Task<CategoryEntity[]> GetCategoriesByIdsAsync(string[] categoriesIds, CategoryResponseGroup respGroup)
        {
            if (categoriesIds == null)
            {
                throw new ArgumentNullException(nameof(categoriesIds));
            }

            if (!categoriesIds.Any())
            {
                return new CategoryEntity[] { };
            }

            if (respGroup.HasFlag(CategoryResponseGroup.WithOutlines))
            {
                respGroup |= CategoryResponseGroup.WithLinks | CategoryResponseGroup.WithParents;
            }

            var result = await Categories.Where(x => categoriesIds.Contains(x.Id)).ToArrayAsync();

            if (respGroup.HasFlag(CategoryResponseGroup.WithLinks))
            {
                var incommingLinksTask = CategoryLinks.Where(x => categoriesIds.Contains(x.TargetCategoryId)).ToArrayAsync();
                var outgoingLinksTask = CategoryLinks.Where(x => categoriesIds.Contains(x.SourceCategoryId)).ToArrayAsync();
                await Task.WhenAll(incommingLinksTask, outgoingLinksTask);
            }

            if (respGroup.HasFlag(CategoryResponseGroup.WithImages))
            {
                var images = await Images.Where(x => categoriesIds.Contains(x.CategoryId)).ToArrayAsync();
            }

            //Load all properties meta information and information for inheritance
            if (respGroup.HasFlag(CategoryResponseGroup.WithProperties))
            {
                //Load category property values by separate query
                var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryValueEntities).Where(x => categoriesIds.Contains(x.CategoryId)).ToArrayAsync();

                var categoryPropertiesIds = await Properties.Where(x => categoriesIds.Contains(x.CategoryId)).Select(x => x.Id).ToArrayAsync();
                var categoryProperties = GetPropertiesByIdsAsync(categoryPropertiesIds);
            }

            return result;
        }

        public async Task<ItemEntity[]> GetItemByIdsAsync(string[] itemIds, ItemResponseGroup respGroup = ItemResponseGroup.ItemLarge)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            if (!itemIds.Any())
            {
                return new ItemEntity[] { };
            }

            // Use breaking query EF performance concept https://msdn.microsoft.com/en-us/data/hh949853.aspx#8
            var retVal = await Items.Include(x => x.Images).Where(x => itemIds.Contains(x.Id)).ToArrayAsync();

            if (respGroup.HasFlag(ItemResponseGroup.Outlines))
            {
                respGroup |= ItemResponseGroup.Links;
            }

            if (respGroup.HasFlag(ItemResponseGroup.ItemProperties))
            {
                var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryValueEntities).Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
            }

            if (respGroup.HasFlag(ItemResponseGroup.Links))
            {
                var relations = await CategoryItemRelations.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
            }

            if (respGroup.HasFlag(ItemResponseGroup.ItemAssets))
            {
                var assets = await Assets.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
            }

            if (respGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
            {
                var editorialReviews = await EditorialReviews.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
            }

            if (respGroup.HasFlag(ItemResponseGroup.Variations))
            {
                // TODO: Call GetItemByIds for variations recursively (need to measure performance and data amount first)

                var variationIds = await Items.Where(x => itemIds.Contains(x.ParentId)).Select(x => x.Id).ToArrayAsync();

                // Always load info, images and property values for variations
                var variationsTask = Items.Include(x => x.Images).Where(x => variationIds.Contains(x.Id)).ToArrayAsync();
                var variationPropertyValuesTask = PropertyValues.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                await Task.WhenAll(variationsTask, variationPropertyValuesTask);

                if (respGroup.HasFlag(ItemResponseGroup.ItemAssets))
                {
                    var variationAssets = await Assets.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                }

                if (respGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
                {
                    var variationEditorialReviews = await EditorialReviews.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                }
            }

            if (respGroup.HasFlag(ItemResponseGroup.ItemAssociations))
            {
                var assosiations = await Associations.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                var assosiatedProductIds = assosiations.Where(x => x.AssociatedItemId != null)
                                                       .Select(x => x.AssociatedItemId).Distinct().ToArray();

                var assosiatedItems = await GetItemByIdsAsync(assosiatedProductIds, ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets);

                var assosiatedCategoryIdsIds = assosiations.Where(x => x.AssociatedCategoryId != null).Select(x => x.AssociatedCategoryId).Distinct().ToArray();
                var associatedCategories = await GetCategoriesByIdsAsync(assosiatedCategoryIdsIds, CategoryResponseGroup.Info | CategoryResponseGroup.WithImages);
            }

            if (respGroup.HasFlag(ItemResponseGroup.ReferencedAssociations))
            {
                var referencedAssociations = await Associations.Where(x => itemIds.Contains(x.AssociatedItemId)).ToArrayAsync();
                var referencedProductIds = referencedAssociations.Select(x => x.ItemId).Distinct().ToArray();
                var referencedProducts = await GetItemByIdsAsync(referencedProductIds, ItemResponseGroup.ItemInfo);
            }

            // Load parents
            var parentIds = retVal.Where(x => x.Parent == null && x.ParentId != null).Select(x => x.ParentId).ToArray();
            var parents = await GetItemByIdsAsync(parentIds, respGroup);

            return retVal;
        }

        public async Task<PropertyEntity[]> GetPropertiesByIdsAsync(string[] propIds, bool loadDictValues = false)
        {
            //Used breaking query EF performance concept https://msdn.microsoft.com/en-us/data/hh949853.aspx#8
            var retVal = await Properties.Where(x => propIds.Contains(x.Id)).ToArrayAsync();

            var propAttributesTask = PropertyAttributes.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
            var propDisplayNamesTask = PropertyDisplayNames.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
            var propValidationRulesTask = PropertyValidationRules.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
            await Task.WhenAll(propAttributesTask, propDisplayNamesTask, propValidationRulesTask);

            if (loadDictValues)
            {
                var propDictionaryItems = await PropertyDictionaryItems.Include(x => x.DictionaryValueEntities).Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
            }
            return retVal;
        }

        /// <summary>
        /// Returned all properties belongs to specified catalog 
        /// For virtual catalog also include all properties for categories linked to this virtual catalog 
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public async Task<PropertyEntity[]> GetAllCatalogPropertiesAsync(string catalogId)
        {
            var retVal = new List<PropertyEntity>();

            var catalog = await Catalogs.FirstOrDefaultAsync(x => x.Id == catalogId);
            if (catalog != null)
            {
                var propertyIds = await Properties.Where(x => x.CatalogId == catalogId).Select(x => x.Id).ToArrayAsync();

                if (catalog.Virtual)
                {
                    //get all category relations
                    var linkedCategoryIds = await CategoryLinks.Where(x => x.TargetCatalogId == catalogId)
                                                         .Select(x => x.SourceCategoryId)
                                                         .Distinct()
                                                         .ToArrayAsync();
                    //linked product categories links
                    var linkedProductCategoryIds = await CategoryItemRelations.Where(x => x.CatalogId == catalogId)
                                                             .Join(Items, link => link.ItemId, item => item.Id, (link, item) => item)
                                                             .Select(x => x.CategoryId)
                                                             .Distinct()
                                                             .ToArrayAsync();
                    linkedCategoryIds = linkedCategoryIds.Concat(linkedProductCategoryIds).Distinct().ToArray();
                    var expandedFlatLinkedCategoryIds = linkedCategoryIds.Concat(await GetAllChildrenCategoriesIdsAsync(linkedCategoryIds)).Distinct().ToArray();

                    propertyIds = propertyIds.Concat(Properties.Where(x => expandedFlatLinkedCategoryIds.Contains(x.CategoryId)).Select(x => x.Id)).Distinct().ToArray();
                    var linkedCatalogIds = await Categories.Where(x => expandedFlatLinkedCategoryIds.Contains(x.Id)).Select(x => x.CatalogId).Distinct().ToArrayAsync();
                    propertyIds = propertyIds.Concat(Properties.Where(x => linkedCatalogIds.Contains(x.CatalogId) && x.CategoryId == null).Select(x => x.Id)).Distinct().ToArray();
                }

                retVal.AddRange(await GetPropertiesByIdsAsync(propertyIds));
            }

            return retVal.ToArray();
        }

        public async Task<PropertyDictionaryItemEntity[]> GetPropertyDictionaryItemsByIdsAsync(string[] dictItemIds)
        {
            if (dictItemIds == null)
            {
                throw new ArgumentNullException(nameof(dictItemIds));
            }
            var result = await PropertyDictionaryItems.Include(x => x.DictionaryValueEntities).Where(x => dictItemIds.Contains(x.Id)).ToArrayAsync();
            return result;
        }

        public async Task<string[]> GetAllChildrenCategoriesIdsAsync(string[] categoryIds)
        {
            string[] result = null;

            if (!categoryIds.IsNullOrEmpty())
            {
                const string commandText = @"
                    WITH cte AS (
                        SELECT a.Id FROM Category a  WHERE Id IN (@CategoryIds)
                        UNION ALL
                        SELECT a.Id FROM Category a JOIN cte c ON a.ParentCategoryId = c.Id
                    )
                    SELECT Id FROM cte WHERE Id NOT IN (@CategoryIds)
                ";
                var parameter = new SqlParameter("@CategoryIds", categoryIds);
                result = await Categories.FromSql(commandText, parameter).Select(x => x.Id).ToArrayAsync();
            }

            return result ?? new string[0];
        }

        public async Task RemoveItemsAsync(string[] itemIds)
        {
            if (!itemIds.IsNullOrEmpty())
            {
                const string commandText = @"
                    DELETE SEO FROM SeoUrlKeyword SEO INNER JOIN Item I ON I.Id = SEO.ObjectId AND SEO.ObjectType = 'CatalogProduct'
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE CR FROM CategoryItemRelation  CR INNER JOIN Item I ON I.Id = CR.ItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)
        
                    DELETE CI FROM CatalogImage CI INNER JOIN Item I ON I.Id = CI.ItemId
                    WHERE I.Id IN (@BatchItemIds)  OR I.ParentId IN (@BatchItemIds)

                    DELETE CA FROM CatalogAsset CA INNER JOIN Item I ON I.Id = CA.ItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE PV FROM PropertyValue PV INNER JOIN Item I ON I.Id = PV.ItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE ER FROM EditorialReview ER INNER JOIN Item I ON I.Id = ER.ItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE A FROM Association A INNER JOIN Item I ON I.Id = A.ItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE A FROM Association A INNER JOIN Item I ON I.Id = A.AssociatedItemId
                    WHERE I.Id IN (@BatchItemIds) OR I.ParentId IN (@BatchItemIds)

                    DELETE  FROM Item  WHERE ParentId IN (@BatchItemIds)

                    DELETE  FROM Item  WHERE Id IN (@BatchItemIds)
                ";

                const int batchSize = 500;
                var skip = 0;
                do
                {
                    var batchItemIds = itemIds.Skip(skip).Take(batchSize).ToArray();
                    await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@BatchItemIds", batchItemIds));

                    skip += batchSize;
                }
                while (skip < itemIds.Length);
                //TODO: Notify about removed entities by event or trigger
            }
        }

        public async Task RemoveCategoriesAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var categoryIds = (await GetAllChildrenCategoriesIdsAsync(ids)).Concat(ids).ToArray();

                var itemIds = Items.Where(i => categoryIds.Contains(i.CategoryId)).Select(i => i.Id).ToArray();
                await RemoveItemsAsync(itemIds);

                const string commandText = @"
                    DELETE FROM SeoUrlKeyword WHERE ObjectType = 'Category' AND ObjectId IN (@CategoryIds)
                    DELETE CI FROM CatalogImage CI INNER JOIN Category C ON C.Id = CI.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE CR FROM CategoryRelation CR INNER JOIN Category C ON C.Id = CR.SourceCategoryId OR C.Id = CR.TargetCategoryId  WHERE C.Id IN (@CategoryIds) 
                    DELETE CIR FROM CategoryItemRelation CIR INNER JOIN Category C ON C.Id = CIR.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE A FROM Association A INNER JOIN Category C ON C.Id = A.AssociatedCategoryId WHERE C.Id IN (@CategoryIds)
                    DELETE P FROM Property P INNER JOIN Category C ON C.Id = P.CategoryId  WHERE C.Id IN (@CategoryIds)
                    DELETE FROM Category WHERE Id IN (@CategoryIds)";

                await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@CategoryIds", categoryIds));

                //TODO: Notify about removed entities by event or trigger
            }
        }

        public async Task RemoveCatalogsAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var itemIds = await Items.Where(i => i.CategoryId == null && ids.Contains(i.CatalogId)).Select(i => i.Id).ToArrayAsync();
                await RemoveItemsAsync(itemIds);

                var categoryIds = await Categories.Where(c => ids.Contains(c.CatalogId)).Select(c => c.Id).ToArrayAsync();
                await RemoveCategoriesAsync(categoryIds);

                const string commandText = @"
                    DELETE CL FROM CatalogLanguage CL INNER JOIN Catalog C ON C.Id = CL.CatalogId WHERE C.Id IN (@Ids)
                    DELETE CR FROM CategoryRelation CR INNER JOIN Catalog C ON C.Id = CR.TargetCatalogId WHERE C.Id IN (@Ids) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId WHERE C.Id IN (@Ids) 
                    DELETE P FROM Property P INNER JOIN Catalog C ON C.Id = P.CatalogId  WHERE C.Id IN (@Ids)
                    DELETE FROM Catalog WHERE Id IN (@Ids)
                ";

                await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@Ids", ids));

                //TODO: Notify about removed entities by event or trigger
            }
        }

        /// <summary>
        /// Delete all exist property values belong to given property.
        /// Because PropertyValue table doesn't have a foreign key to Property table by design,
        /// we use columns Name and TargetType to find values that reference to the deleting property.  
        /// </summary>
        /// <param name="propertyId"></param>
        public async Task RemoveAllPropertyValuesAsync(string propertyId)
        {
            var properties = await GetPropertiesByIdsAsync(new[] { propertyId });
            var catalogProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Catalog.ToString()));
            var categoryProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Category.ToString()));
            var itemProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Product.ToString()) || x.TargetType.EqualsInvariant(PropertyType.Variation.ToString()));

            string commandText;
            if (catalogProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId AND C.Id = '@CatalogId' WHERE PV.Name = '@Name'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@CatalogId", catalogProperty.CatalogId), new SqlParameter("@Name", catalogProperty.Name));
            }
            if (categoryProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId AND C.CatalogId = '@CatalogId' WHERE PV.Name = '@Name'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@CatalogId", categoryProperty.CatalogId), new SqlParameter("@Name", categoryProperty.Name));
            }
            if (itemProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Item I ON I.Id = PV.ItemId AND I.CatalogId = '@CatalogId' WHERE PV.Name = '@Name'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText, new SqlParameter("@CatalogId", itemProperty.CatalogId), new SqlParameter("@Name", itemProperty.Name));
            }
        }

        public GenericSearchResult<AssociationEntity> SearchAssociations(ProductAssociationSearchCriteria criteria)
        {
            //TODO
            throw new NotImplementedException();
        }

        #endregion
    }

}
