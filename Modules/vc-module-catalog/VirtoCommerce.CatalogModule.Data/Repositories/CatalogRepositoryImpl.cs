using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Model.Search;
using VirtoCommerce.CatalogModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Extensions;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.CatalogModule.Data.Repositories
{
    public class CatalogRepositoryImpl : DbContextRepositoryBase<CatalogDbContext>, ICatalogRepository
    {
        public CatalogRepositoryImpl(CatalogDbContext dbContext)
            : base(dbContext)
        {
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
        public IQueryable<SeoInfoEntity> SeoInfos => DbContext.Set<SeoInfoEntity>();

        public async Task<CatalogEntity[]> GetCatalogsByIdsAsync(string[] catalogIds)
        {
            var result = Array.Empty<CatalogEntity>();

            if (!catalogIds.IsNullOrEmpty())
            {
                result = await Catalogs.Include(x => x.CatalogLanguages)
                    .Include(x => x.IncomingLinks)
                    .Where(x => catalogIds.Contains(x.Id))
                    .ToArrayAsync();

                if (result.Any())
                {
                    var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryItemValues).Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null).ToArrayAsync();
                    var catalogPropertiesIds = await Properties.Where(x => catalogIds.Contains(x.CatalogId) && x.CategoryId == null)
                        .Select(x => x.Id)
                        .ToArrayAsync();
                    var catalogProperties = await GetPropertiesByIdsAsync(catalogPropertiesIds);
                }
            }

            return result;
        }

        public async Task<CategoryEntity[]> GetCategoriesByIdsAsync(string[] categoriesIds, string responseGroup = null)
        {
            var categoryResponseGroup = EnumUtility.SafeParseFlags(responseGroup, CategoryResponseGroup.Full);
            var result = Array.Empty<CategoryEntity>();

            if (!categoriesIds.IsNullOrEmpty())
            {
                result = await Categories.Where(x => categoriesIds.Contains(x.Id)).ToArrayAsync();

                if (result.Any())
                {
                    if (categoryResponseGroup.HasFlag(CategoryResponseGroup.WithOutlines))
                    {
                        categoryResponseGroup |= CategoryResponseGroup.WithLinks | CategoryResponseGroup.WithParents;
                    }

                    if (categoryResponseGroup.HasFlag(CategoryResponseGroup.WithLinks))
                    {
                        var incomingLinksTask = CategoryLinks.Where(x => categoriesIds.Contains(x.TargetCategoryId))
                            .ToArrayAsync();
                        var outgoingLinksTask = CategoryLinks.Where(x => categoriesIds.Contains(x.SourceCategoryId))
                            .ToArrayAsync();
                        await Task.WhenAll(incomingLinksTask, outgoingLinksTask);
                    }

                    if (categoryResponseGroup.HasFlag(CategoryResponseGroup.WithImages))
                    {
                        var images = await Images.Where(x => categoriesIds.Contains(x.CategoryId)).ToArrayAsync();
                    }

                    if (categoryResponseGroup.HasFlag(CategoryResponseGroup.WithSeo))
                    {
                        var seoInfos = await SeoInfos.Where(x => categoriesIds.Contains(x.CategoryId)).ToArrayAsync();
                    }

                    //Load all properties meta information and information for inheritance
                    if (categoryResponseGroup.HasFlag(CategoryResponseGroup.WithProperties))
                    {
                        //Load category property values by separate query
                        var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryItemValues)
                            .Where(x => categoriesIds.Contains(x.CategoryId)).ToArrayAsync();

                        var categoryPropertiesIds = await Properties.Where(x => categoriesIds.Contains(x.CategoryId))
                            .Select(x => x.Id).ToArrayAsync();
                        var categoryProperties = await GetPropertiesByIdsAsync(categoryPropertiesIds);
                    }
                }
            }

            return result;
        }

        public async Task<ItemEntity[]> GetItemByIdsAsync(string[] itemIds, string responseGroup = null)
        {
            var itemResponseGroup = EnumUtility.SafeParseFlags(responseGroup, ItemResponseGroup.ItemLarge);
            var result = Array.Empty<ItemEntity>();

            if (!itemIds.IsNullOrEmpty())
            {
                // Use breaking query EF performance concept https://docs.microsoft.com/en-us/ef/ef6/fundamentals/performance/perf-whitepaper#8-loading-related-entities
                result = await Items.Include(x => x.Images).Where(x => itemIds.Contains(x.Id)).ToArrayAsync();

                if (result.Any())
                {
                    if (itemResponseGroup.HasFlag(ItemResponseGroup.Outlines))
                    {
                        itemResponseGroup |= ItemResponseGroup.Links;
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemProperties))
                    {
                        var propertyValues = await PropertyValues.Include(x => x.DictionaryItem.DictionaryItemValues).Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.Links))
                    {
                        var relations = await CategoryItemRelations.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemAssets))
                    {
                        var assets = await Assets.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
                    {
                        var editorialReviews = await EditorialReviews.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.WithSeo))
                    {
                        var seoInfos = await SeoInfos.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.Variations))
                    {
                        // TODO: Call GetItemByIds for variations recursively (need to measure performance and data amount first)

                        var variationIds = await Items.Where(x => itemIds.Contains(x.ParentId)).Select(x => x.Id).ToArrayAsync();

                        // Always load info, images and property values for variations
                        var variationsTask = Items.Include(x => x.Images).Where(x => variationIds.Contains(x.Id)).ToArrayAsync();
                        var variationPropertyValuesTask = PropertyValues.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                        await Task.WhenAll(variationsTask, variationPropertyValuesTask);

                        if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemAssets))
                        {
                            var variationAssets = await Assets.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                        }

                        if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemEditorialReviews))
                        {
                            var variationEditorialReviews = await EditorialReviews.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                        }

                        if (itemResponseGroup.HasFlag(ItemResponseGroup.Seo))
                        {
                            var variationsSeoInfos = await SeoInfos.Where(x => variationIds.Contains(x.ItemId)).ToArrayAsync();
                        }
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.ItemAssociations))
                    {
                        var associations = await Associations.Where(x => itemIds.Contains(x.ItemId)).ToArrayAsync();
                        var associatedProductIds = associations.Where(x => x.AssociatedItemId != null)
                            .Select(x => x.AssociatedItemId).Distinct().ToArray();

                        var associatedItems = await GetItemByIdsAsync(associatedProductIds, (ItemResponseGroup.ItemInfo | ItemResponseGroup.ItemAssets).ToString());

                        var associatedCategoryIdsIds = associations.Where(x => x.AssociatedCategoryId != null).Select(x => x.AssociatedCategoryId).Distinct().ToArray();
                        var associatedCategories = await GetCategoriesByIdsAsync(associatedCategoryIdsIds, (CategoryResponseGroup.Info | CategoryResponseGroup.WithImages).ToString());
                    }

                    if (itemResponseGroup.HasFlag(ItemResponseGroup.ReferencedAssociations))
                    {
                        var referencedAssociations = await Associations.Where(x => itemIds.Contains(x.AssociatedItemId)).ToArrayAsync();
                        var referencedProductIds = referencedAssociations.Select(x => x.ItemId).Distinct().ToArray();
                        var referencedProducts = await GetItemByIdsAsync(referencedProductIds, ItemResponseGroup.ItemInfo.ToString());
                    }

                    // Load parents
                    var parentIds = result.Where(x => x.Parent == null && x.ParentId != null).Select(x => x.ParentId).ToArray();
                    var parents = await GetItemByIdsAsync(parentIds, responseGroup);
                }
            }

            return result;
        }

        public async Task<PropertyEntity[]> GetPropertiesByIdsAsync(string[] propIds, bool loadDictValues = false)
        {
            var result = Array.Empty<PropertyEntity>();

            if (!propIds.IsNullOrEmpty())
            {
                //Used breaking query EF performance concept https://msdn.microsoft.com/en-us/data/hh949853.aspx#8
                result = await Properties.Where(x => propIds.Contains(x.Id)).ToArrayAsync();

                if (result.Any())
                {
                    var propAttributesTask = PropertyAttributes.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
                    var propDisplayNamesTask = PropertyDisplayNames.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
                    var propValidationRulesTask = PropertyValidationRules.Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
                    await Task.WhenAll(propAttributesTask, propDisplayNamesTask, propValidationRulesTask);

                    if (loadDictValues)
                    {
                        var propDictionaryItems = await PropertyDictionaryItems.Include(x => x.DictionaryItemValues).Where(x => propIds.Contains(x.PropertyId)).ToArrayAsync();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returned all properties belongs to specified catalog 
        /// For virtual catalog also include all properties for categories linked to this virtual catalog 
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public async Task<PropertyEntity[]> GetAllCatalogPropertiesAsync(string catalogId)
        {
            var result = Array.Empty<PropertyEntity>();

            if (!string.IsNullOrEmpty(catalogId))
            {
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

                    result = await GetPropertiesByIdsAsync(propertyIds);
                }
            }

            return result;
        }

        public async Task<PropertyDictionaryItemEntity[]> GetPropertyDictionaryItemsByIdsAsync(string[] dictItemIds)
        {
            var result = Array.Empty<PropertyDictionaryItemEntity>();

            if (!dictItemIds.IsNullOrEmpty())
            {
                result = await PropertyDictionaryItems.Include(x => x.DictionaryItemValues)
                    .Where(x => dictItemIds.Contains(x.Id))
                    .ToArrayAsync();
            }

            return result;
        }

        public async Task<string[]> GetAllChildrenCategoriesIdsAsync(string[] categoryIds)
        {
            string[] result = null;

            if (!categoryIds.IsNullOrEmpty())
            {
                const string commandTemplate = @"
                    WITH cte AS (
                        SELECT a.Id FROM Category a  WHERE Id IN ({0})
                        UNION ALL
                        SELECT a.Id FROM Category a JOIN cte c ON a.ParentCategoryId = c.Id
                    )
                    SELECT Id FROM cte WHERE Id NOT IN ({0})
                ";

                var getAllChildrenCategoriesCommand = CreateCommand(commandTemplate, categoryIds);
                result = await DbContext.ExecuteArrayAsync<string>(getAllChildrenCategoriesCommand.Text, getAllChildrenCategoriesCommand.Parameters);
            }

            return result ?? new string[0];
        }

        public async Task RemoveItemsAsync(string[] itemIds)
        {
            if (!itemIds.IsNullOrEmpty())
            {
                const int batchSize = 500;
                var skip = 0;
                do
                {
                    const string commandTemplate = @"
                        DELETE SEO FROM CatalogSeoInfo SEO INNER JOIN Item I ON I.Id = SEO.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE CR FROM CategoryItemRelation  CR INNER JOIN Item I ON I.Id = CR.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})
        
                        DELETE CI FROM CatalogImage CI INNER JOIN Item I ON I.Id = CI.ItemId
                        WHERE I.Id IN ({0})  OR I.ParentId IN ({0})

                        DELETE CA FROM CatalogAsset CA INNER JOIN Item I ON I.Id = CA.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE PV FROM PropertyValue PV INNER JOIN Item I ON I.Id = PV.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE ER FROM EditorialReview ER INNER JOIN Item I ON I.Id = ER.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE A FROM Association A INNER JOIN Item I ON I.Id = A.ItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE A FROM Association A INNER JOIN Item I ON I.Id = A.AssociatedItemId
                        WHERE I.Id IN ({0}) OR I.ParentId IN ({0})

                        DELETE  FROM Item  WHERE ParentId IN ({0})

                        DELETE  FROM Item  WHERE Id IN ({0})
                    ";

                    await ExecuteStoreQueryAsync(commandTemplate, itemIds);

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

                var itemIds = await Items.Where(i => categoryIds.Contains(i.CategoryId)).Select(i => i.Id).ToArrayAsync();
                await RemoveItemsAsync(itemIds);

                const string commandTemplate = @"
                    DELETE SEO FROM CatalogSeoInfo SEO INNER JOIN Category C ON C.Id = SEO.CategoryId WHERE C.Id IN ({0}) 
                    DELETE CI FROM CatalogImage CI INNER JOIN Category C ON C.Id = CI.CategoryId WHERE C.Id IN ({0}) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId WHERE C.Id IN ({0}) 
                    DELETE CR FROM CategoryRelation CR INNER JOIN Category C ON C.Id = CR.SourceCategoryId OR C.Id = CR.TargetCategoryId  WHERE C.Id IN ({0}) 
                    DELETE CIR FROM CategoryItemRelation CIR INNER JOIN Category C ON C.Id = CIR.CategoryId WHERE C.Id IN ({0}) 
                    DELETE A FROM Association A INNER JOIN Category C ON C.Id = A.AssociatedCategoryId WHERE C.Id IN ({0})
                    DELETE P FROM Property P INNER JOIN Category C ON C.Id = P.CategoryId  WHERE C.Id IN ({0})
                    DELETE FROM Category WHERE Id IN ({0})
                ";

                await ExecuteStoreQueryAsync(commandTemplate, categoryIds);

                //TODO: Notify about removed entities by event or trigger
            }
        }

        public async Task RemoveCatalogsAsync(string[] ids)
        {
            if (!ids.IsNullOrEmpty())
            {
                var itemIds = await Items.Where(i => ids.Contains(i.CatalogId)).Select(i => i.Id).ToArrayAsync();
                await RemoveItemsAsync(itemIds);

                var categoryIds = await Categories.Where(c => ids.Contains(c.CatalogId)).Select(c => c.Id).ToArrayAsync();
                await RemoveCategoriesAsync(categoryIds);

                const string commandTemplate = @"
                    DELETE CL FROM CatalogLanguage CL INNER JOIN Catalog C ON C.Id = CL.CatalogId WHERE C.Id IN ({0})
                    DELETE CR FROM CategoryRelation CR INNER JOIN Catalog C ON C.Id = CR.TargetCatalogId WHERE C.Id IN ({0}) 
                    DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId WHERE C.Id IN ({0}) 
                    DELETE P FROM Property P INNER JOIN Catalog C ON C.Id = P.CatalogId  WHERE C.Id IN ({0})
                    DELETE FROM Catalog WHERE Id IN ({0})
                ";

                await ExecuteStoreQueryAsync(commandTemplate, ids);

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
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Catalog C ON C.Id = PV.CatalogId AND C.Id = '{catalogProperty.CatalogId}' WHERE PV.Name = '{catalogProperty.Name}'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText);
            }
            if (categoryProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Category C ON C.Id = PV.CategoryId AND C.CatalogId = '{categoryProperty.CatalogId}' WHERE PV.Name = '{categoryProperty.Name}'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText);
            }
            if (itemProperty != null)
            {
                commandText = $"DELETE PV FROM PropertyValue PV INNER JOIN Item I ON I.Id = PV.ItemId AND I.CatalogId = '{itemProperty.CatalogId}' WHERE PV.Name = '{itemProperty.Name}'";
                await DbContext.Database.ExecuteSqlCommandAsync(commandText);
            }
        }

        public async Task<GenericSearchResult<AssociationEntity>> SearchAssociations(ProductAssociationSearchCriteria criteria)
        {
            var result = new GenericSearchResult<AssociationEntity>();

            var countSqlCommandText = @"
                ;WITH Association_CTE AS
                (
                    SELECT *
                    FROM Association
                    WHERE ItemId IN ({0})
                "
                                      + (!string.IsNullOrEmpty(criteria.Group) ? $" AND AssociationType = @group" : string.Empty) +
                                      @"), Category_CTE AS
                (
                    SELECT AssociatedCategoryId Id
                    FROM Association_CTE
                    WHERE AssociatedCategoryId IS NOT NULL
                    UNION ALL
                    SELECT c.Id
                    FROM Category c
                    INNER JOIN Category_CTE cte ON c.ParentCategoryId = cte.Id
                ),
                Item_CTE AS 
                (
                    SELECT  i.Id
                    FROM (SELECT DISTINCT Id FROM Category_CTE) c
                    LEFT JOIN Item i ON c.Id=i.CategoryId WHERE i.ParentId IS NULL
                    UNION
                    SELECT AssociatedItemId Id FROM Association_CTE
                ) 
                SELECT COUNT(Id) FROM Item_CTE";

            //var groupString = "";
            var querySqlCommandText = new StringBuilder();
            querySqlCommandText.Append(@"
                    ;WITH Association_CTE AS
                    (
                        SELECT
                            Id	
                            ,AssociationType
                            ,Priority
                            ,ItemId
                            ,CreatedDate
                            ,ModifiedDate
                            ,CreatedBy
                            ,ModifiedBy
                            ,AssociatedItemId
                            ,AssociatedCategoryId
                            ,Tags
                            ,Quantity
                            ,OuterId
                        FROM Association
                        WHERE ItemId IN({0})")
                ;

            if (!string.IsNullOrEmpty(criteria.Group))
            {
                querySqlCommandText.Append(" AND AssociationType = @group");
            }

            querySqlCommandText.Append(@"), Category_CTE AS
                    (
                        SELECT AssociatedCategoryId Id, AssociatedCategoryId
                        FROM Association_CTE
                        WHERE AssociatedCategoryId IS NOT NULL
                        UNION ALL
                        SELECT c.Id, cte.AssociatedCategoryId
                        FROM Category c
                        INNER JOIN Category_CTE cte ON c.ParentCategoryId = cte.Id
                    ),
                    Item_CTE AS 
                    (
                        SELECT 
                            a.Id	
                            ,a.AssociationType
                            ,a.Priority
                            ,a.ItemId
                            ,a.CreatedDate
                            ,a.ModifiedDate
                            ,a.CreatedBy
                            ,a.ModifiedBy
                            ,i.Id AssociatedItemId
                            ,a.AssociatedCategoryId
                            ,a.Tags
                            ,a.Quantity
                            ,a.OuterId
                        FROM Category_CTE cat
                        LEFT JOIN Item i ON cat.Id=i.CategoryId
                        LEFT JOIN Association a ON cat.AssociatedCategoryId=a.AssociatedCategoryId
                        WHERE i.ParentId IS NULL
                        UNION
                        SELECT * FROM Association_CTE
                    ) 
                    SELECT  * FROM Item_CTE WHERE AssociatedItemId IS NOT NULL ORDER BY Priority ");
            querySqlCommandText.Append($"OFFSET {criteria.Skip} ROWS FETCH NEXT {criteria.Take} ROWS ONLY");


            var countSqlCommand = CreateCommand(countSqlCommandText, criteria.ObjectIds);
            var querySqlCommand = CreateCommand(querySqlCommandText.ToString(), criteria.ObjectIds);
            if (!string.IsNullOrEmpty(criteria.Group))
            {
                countSqlCommand.Parameters = countSqlCommand.Parameters.Concat(new[] { new SqlParameter($"@group", criteria.Group) }).ToArray();
                querySqlCommand.Parameters = querySqlCommand.Parameters.Concat(new[] { new SqlParameter($"@group", criteria.Group) }).ToArray();
            }

            result.TotalCount = await DbContext.ExecuteScalarAsync<int>(countSqlCommand.Text, countSqlCommand.Parameters);
            result.Results = await Associations.FromSql(querySqlCommand.Text, querySqlCommand.Parameters).ToListAsync();

            return result;
        }

        #endregion

        protected virtual async Task<int> ExecuteStoreQueryAsync(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var command = CreateCommand(commandTemplate, parameterValues);
            return await DbContext.Database.ExecuteSqlCommandAsync(command.Text, command.Parameters);
        }

        protected virtual Command CreateCommand(string commandTemplate, IEnumerable<string> parameterValues)
        {
            var parameters = parameterValues.Select((v, i) => new SqlParameter($"@p{i}", v)).ToArray();
            var parameterNames = string.Join(",", parameters.Select(p => p.ParameterName));

            return new Command
            {
                Text = string.Format(commandTemplate, parameterNames),
                Parameters = parameters.OfType<object>().ToArray(),
            };
        }

        protected class Command
        {
            public string Text { get; set; }
            public object[] Parameters { get; set; }
        }
    }
}
