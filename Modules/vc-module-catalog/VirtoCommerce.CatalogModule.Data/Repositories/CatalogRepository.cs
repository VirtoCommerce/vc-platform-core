using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.Domain.Catalog.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using coreModel = VirtoCommerce.Domain.Catalog.Model;
using dataModel = VirtoCommerce.CatalogModule.Data.Model;

namespace VirtoCommerce.CatalogModule.Data.Repositories
{
    public class CatalogRepository : DbContextRepositoryBase<CatalogDbContext>, ICatalogRepository
    {
        public CatalogRepository(CatalogDbContext dbContext)
         : base(dbContext)
        {
        }

        #region ICatalogRepository Members

        public IQueryable<dataModel.CategoryEntity> Categories => DbContext.Set<dataModel.CategoryEntity>();
        public IQueryable<dataModel.CatalogEntity> Catalogs => DbContext.Set<dataModel.CatalogEntity>();
        public IQueryable<dataModel.PropertyValueEntity> PropertyValues => DbContext.Set<dataModel.PropertyValueEntity>();
        public IQueryable<dataModel.ImageEntity> Images => DbContext.Set<dataModel.ImageEntity>();
        public IQueryable<dataModel.AssetEntity> Assets => DbContext.Set<dataModel.AssetEntity>();
        public IQueryable<dataModel.ItemEntity> Items => DbContext.Set<dataModel.ItemEntity>();
        public IQueryable<dataModel.EditorialReviewEntity> EditorialReviews => DbContext.Set<dataModel.EditorialReviewEntity>();
        public IQueryable<dataModel.PropertyEntity> Properties => DbContext.Set<dataModel.PropertyEntity>();
        public IQueryable<dataModel.PropertyDictionaryValueEntity> PropertyDictionaryValues => DbContext.Set<dataModel.PropertyDictionaryValueEntity>();
        public IQueryable<dataModel.PropertyDisplayNameEntity> PropertyDisplayNames => DbContext.Set<dataModel.PropertyDisplayNameEntity>();
        public IQueryable<dataModel.PropertyAttributeEntity> PropertyAttributes => DbContext.Set<dataModel.PropertyAttributeEntity>();
        public IQueryable<dataModel.CategoryItemRelationEntity> CategoryItemRelations => DbContext.Set<dataModel.CategoryItemRelationEntity>();
        public IQueryable<dataModel.AssociationEntity> Associations => DbContext.Set<dataModel.AssociationEntity>();
        public IQueryable<dataModel.CategoryRelationEntity> CategoryLinks => DbContext.Set<dataModel.CategoryRelationEntity>();
        public IQueryable<dataModel.PropertyValidationRuleEntity> PropertyValidationRules => DbContext.Set<dataModel.PropertyValidationRuleEntity>();

        public dataModel.CatalogEntity[] GetCatalogsByIds(string[] catalogIds)
        {
            var retVal = Catalogs.Include(x => x.CatalogLanguages)
                                 .Include(x => x.IncommingLinks)
                                 .Where(x => catalogIds.Contains(x.Id))
                                 .ToArray();

            var propertyValues = PropertyValues.Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null).ToArray();
            var catalogPropertiesIds = Properties.Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null)
                                                 .Select(x => x.Id)
                                                 .ToArray();
            var catalogProperties = GetPropertiesByIds(catalogPropertiesIds);

            return retVal;
        }

        public dataModel.CategoryEntity[] GetCategoriesByIds(string[] categoriesIds, coreModel.CategoryResponseGroup respGroup)
        {
            if (categoriesIds == null)
            {
                throw new ArgumentNullException(nameof(categoriesIds));
            }

            if (!categoriesIds.Any())
            {
                return new dataModel.CategoryEntity[] { };
            }

            if (respGroup.HasFlag(coreModel.CategoryResponseGroup.WithOutlines))
            {
                respGroup |= coreModel.CategoryResponseGroup.WithLinks | coreModel.CategoryResponseGroup.WithParents;
            }

            var result = Categories.Where(x => categoriesIds.Contains(x.Id)).ToArray();

            if (respGroup.HasFlag(coreModel.CategoryResponseGroup.WithLinks))
            {
                var incommingLinks = CategoryLinks.Where(x => categoriesIds.Contains(x.TargetCategoryId)).ToArray();
                var outgoingLinks = CategoryLinks.Where(x => categoriesIds.Contains(x.SourceCategoryId)).ToArray();
            }

            if (respGroup.HasFlag(coreModel.CategoryResponseGroup.WithImages))
            {
                var images = Images.Where(x => categoriesIds.Contains(x.CategoryId)).ToArray();
            }

            //Load all properties meta information and information for inheritance
            if (respGroup.HasFlag(coreModel.CategoryResponseGroup.WithProperties))
            {
                //Load category property values by separate query
                var propertyValues = PropertyValues.Where(x => categoriesIds.Contains(x.CategoryId)).ToArray();

                var categoryPropertiesIds = Properties.Where(x => categoriesIds.Contains(x.CategoryId)).Select(x => x.Id).ToArray();
                var categoryProperties = GetPropertiesByIds(categoryPropertiesIds);
            }

            return result;
        }

        public dataModel.ItemEntity[] GetItemByIds(string[] itemIds, coreModel.ItemResponseGroup respGroup = coreModel.ItemResponseGroup.ItemLarge)
        {
            if (itemIds == null)
            {
                throw new ArgumentNullException(nameof(itemIds));
            }

            if (!itemIds.Any())
            {
                return new dataModel.ItemEntity[] { };
            }

            // Use breaking query EF performance concept https://msdn.microsoft.com/en-us/data/hh949853.aspx#8
            var retVal = Items.Include(x => x.Images).Where(x => itemIds.Contains(x.Id)).ToArray();            

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.Outlines))
            {
                respGroup |= coreModel.ItemResponseGroup.Links;
            }

            if(respGroup.HasFlag(coreModel.ItemResponseGroup.ItemProperties))
            {
                var propertyValues = PropertyValues.Where(x => itemIds.Contains(x.ItemId)).ToArray();
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.Links))
            {
                var relations = CategoryItemRelations.Where(x => itemIds.Contains(x.ItemId)).ToArray();
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.ItemAssets))
            {
                var assets = Assets.Where(x => itemIds.Contains(x.ItemId)).ToArray();
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.ItemEditorialReviews))
            {
                var editorialReviews = EditorialReviews.Where(x => itemIds.Contains(x.ItemId)).ToArray();
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.Variations))
            {
                // TODO: Call GetItemByIds for variations recursively (need to measure performance and data amount first)

                var variationIds = Items.Where(x => itemIds.Contains(x.ParentId)).Select(x => x.Id).ToArray();

                // Always load info, images and property values for variations
                var variations = Items.Include(x => x.Images).Where(x => variationIds.Contains(x.Id)).ToArray();
                var variationPropertyValues = PropertyValues.Where(x => variationIds.Contains(x.ItemId)).ToArray();

                if (respGroup.HasFlag(coreModel.ItemResponseGroup.ItemAssets))
                {
                    var variationAssets = Assets.Where(x => variationIds.Contains(x.ItemId)).ToArray();
                }

                if (respGroup.HasFlag(coreModel.ItemResponseGroup.ItemEditorialReviews))
                {
                    var variationEditorialReviews = EditorialReviews.Where(x => variationIds.Contains(x.ItemId)).ToArray();
                }
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.ItemAssociations))
            {
                var assosiations = Associations.Where(x => itemIds.Contains(x.ItemId)).ToArray();
                var assosiatedProductIds = assosiations.Where(x => x.AssociatedItemId != null)
                                                       .Select(x => x.AssociatedItemId).Distinct().ToArray();

                var assosiatedItems = GetItemByIds(assosiatedProductIds, coreModel.ItemResponseGroup.ItemInfo | coreModel.ItemResponseGroup.ItemAssets);

                var assosiatedCategoryIdsIds = assosiations.Where(x => x.AssociatedCategoryId != null).Select(x => x.AssociatedCategoryId).Distinct().ToArray();
                var associatedCategories = GetCategoriesByIds(assosiatedCategoryIdsIds, coreModel.CategoryResponseGroup.Info);
            }

            if (respGroup.HasFlag(coreModel.ItemResponseGroup.ReferencedAssociations))
            {
                var referencedAssociations = Associations.Where(x => itemIds.Contains(x.AssociatedItemId)).ToArray();
                var referencedProductIds = referencedAssociations.Select(x => x.ItemId).Distinct().ToArray();
                var referencedProducts = GetItemByIds(referencedProductIds, coreModel.ItemResponseGroup.ItemInfo);
            }

            // Load parents
            var parentIds = retVal.Where(x => x.Parent == null && x.ParentId != null).Select(x => x.ParentId).ToArray();
            var parents = GetItemByIds(parentIds, respGroup);

            return retVal;
        }

        public dataModel.PropertyEntity[] GetPropertiesByIds(string[] propIds)
        {
            //Used breaking query EF performance concept https://msdn.microsoft.com/en-us/data/hh949853.aspx#8
            var retVal = Properties.Where(x => propIds.Contains(x.Id)).ToArray();

            var propAttributes = PropertyAttributes.Where(x => propIds.Contains(x.PropertyId)).ToArray();
            var propDisplayNames = PropertyDisplayNames.Where(x => propIds.Contains(x.PropertyId)).ToArray();
            var propValidationRules = PropertyValidationRules.Where(x => propIds.Contains(x.PropertyId)).ToArray();

            //Do not load dictionary values for not enum properties
            var dictPropertiesIds = retVal.Where(x => x.IsEnum).Select(x => x.Id).ToArray();
            if (!dictPropertiesIds.IsNullOrEmpty())
            {
                var dictValues = PropertyDictionaryValues.Where(x => dictPropertiesIds.Contains(x.PropertyId)).ToArray();
            }

            return retVal;
        }

        /// <summary>
        /// Returned all properties belongs to specified catalog 
        /// For virtual catalog also include all properties for categories linked to this virtual catalog 
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public dataModel.PropertyEntity[] GetAllCatalogProperties(string catalogId)
        {
            var retVal = new List<dataModel.PropertyEntity>();

            var catalog = Catalogs.FirstOrDefault(x => x.Id == catalogId);
            if (catalog != null)
            {
                var propertyIds = Properties.Where(x => x.CatalogId == catalogId).Select(x => x.Id).ToArray();

                if (catalog.Virtual)
                {
                    //get all category relations
                    var linkedCategoryIds = CategoryLinks.Where(x => x.TargetCatalogId == catalogId)
                                                         .Select(x => x.SourceCategoryId)
                                                         .Distinct()
                                                         .ToArray();
                    //linked product categories links
                    var linkedProductCategoryIds = CategoryItemRelations.Where(x => x.CatalogId == catalogId)
                                                             .Join(Items, link => link.ItemId, item => item.Id, (link, item) => item)
                                                             .Select(x => x.CategoryId)
                                                             .Distinct()
                                                             .ToArray();
                    linkedCategoryIds = linkedCategoryIds.Concat(linkedProductCategoryIds).Distinct().ToArray();
                    var expandedFlatLinkedCategoryIds = linkedCategoryIds.Concat(GetAllChildrenCategoriesIds(linkedCategoryIds)).Distinct().ToArray();

                    propertyIds = propertyIds.Concat(Properties.Where(x => expandedFlatLinkedCategoryIds.Contains(x.CategoryId)).Select(x => x.Id)).Distinct().ToArray();
                    var linkedCatalogIds = Categories.Where(x => expandedFlatLinkedCategoryIds.Contains(x.Id)).Select(x => x.CatalogId).Distinct().ToArray();
                    propertyIds = propertyIds.Concat(Properties.Where(x => linkedCatalogIds.Contains(x.CatalogId) && x.CategoryId == null).Select(x => x.Id)).Distinct().ToArray();
                }

                retVal.AddRange(GetPropertiesByIds(propertyIds));
            }

            return retVal.ToArray();
        }

        public string[] GetAllChildrenCategoriesIds(string[] categoryIds)
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
                result = Categories.FromSql(commandText, parameter).Select(x => x.Id).ToArray();
            }

            return result ?? new string[0];
        }

        public void RemoveItems(string[] itemIds)
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
                    DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@BatchItemIds", batchItemIds));

                    skip += batchSize;
                }
                while (skip < itemIds.Length);
                //TODO: Notify about removed entities by event or trigger
            }
        }

        public void RemoveCategories(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var categoryIds = GetAllChildrenCategoriesIds(ids).Concat(ids).ToArray();

                var itemIds = Items.Where(i => categoryIds.Contains(i.CategoryId)).Select(i => i.Id).ToArray();
                RemoveItems(itemIds);

                const string commandText = @"
                    DELETE FROM SeoUrlKeyword WHERE ObjectType = 'Category' AND ObjectId IN (@CategoryIds)
                    DELETE CI FROM CatalogImage CI INNER JOIN Category C ON C.Id = CI.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE CR FROM CategoryRelation CR INNER JOIN Category C ON C.Id = CR.SourceCategoryId OR C.Id = CR.TargetCategoryId  WHERE C.Id IN (@CategoryIds) 
                    DELETE CIR FROM CategoryItemRelation CIR INNER JOIN Category C ON C.Id = CIR.CategoryId WHERE C.Id IN (@CategoryIds) 
                    DELETE A FROM Association A INNER JOIN Category C ON C.Id = A.AssociatedCategoryId WHERE C.Id IN (@CategoryIds)
                    DELETE P FROM Property P INNER JOIN Category C ON C.Id = P.CategoryId  WHERE C.Id IN (@CategoryIds)
                    DELETE FROM Category WHERE Id IN (@CategoryIds)";

                DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@CategoryIds", categoryIds));

                //TODO: Notify about removed entities by event or trigger
            }
        }

        public void RemoveCatalogs(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var itemIds = Items.Where(i => i.CategoryId == null && ids.Contains(i.CatalogId)).Select(i => i.Id).ToArray();
                RemoveItems(itemIds);

                var categoryIds = Categories.Where(c => ids.Contains(c.CatalogId)).Select(c => c.Id).ToArray();
                RemoveCategories(categoryIds);

                const string commandText = @"
                    DELETE CL FROM CatalogLanguage CL INNER JOIN Catalog C ON C.Id = CL.CatalogId WHERE C.Id IN (@Ids)
                    DELETE CR FROM CategoryRelation CR INNER JOIN Catalog C ON C.Id = CR.TargetCatalogId WHERE C.Id IN (@Ids) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId WHERE C.Id IN (@Ids) 
                    DELETE P FROM Property P INNER JOIN Catalog C ON C.Id = P.CatalogId  WHERE C.Id IN (@Ids)
                    DELETE FROM Catalog WHERE Id IN (@Ids)
                ";

                DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@Ids", ids));

                //TODO: Notify about removed entities by event or trigger
            }
        }

        /// <summary>
        /// Delete all exist property values belong to given property.
        /// Because PropertyValue table doesn't have a foreign key to Property table by design,
        /// we use columns Name and TargetType to find values that reference to the deleting property.  
        /// </summary>
        /// <param name="propertyId"></param>
        public void RemoveAllPropertyValues(string propertyId)
        {
            var properties = GetPropertiesByIds(new[] { propertyId });          
            var catalogProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Catalog.ToString()));
            var categoryProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Category.ToString()));
            var itemProperty = properties.FirstOrDefault(x => x.TargetType.EqualsInvariant(PropertyType.Product.ToString()) || x.TargetType.EqualsInvariant(PropertyType.Variation.ToString()));

            string commandText;
            if (catalogProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId AND C.Id = '@CatalogId' WHERE PV.Name = '@Name'";
                DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@CatalogId", catalogProperty.CatalogId), new SqlParameter("@Name", catalogProperty.Name));
            }          
            if (categoryProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId AND C.CatalogId = '@CatalogId' WHERE PV.Name = '@Name'";
                DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@CatalogId", categoryProperty.CatalogId), new SqlParameter("@Name", categoryProperty.Name));
            }
            if (itemProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Item I ON I.Id = PV.ItemId AND I.CatalogId = '@CatalogId' WHERE PV.Name = '@Name'";
                DbContext.Database.ExecuteSqlCommand(commandText, new SqlParameter("@CatalogId", itemProperty.CatalogId), new SqlParameter("@Name", itemProperty.Name));
            }
        }
        #endregion


    }
}
