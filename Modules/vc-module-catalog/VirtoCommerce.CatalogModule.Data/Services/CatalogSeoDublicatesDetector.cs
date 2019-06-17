using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.CatalogModule.Core.Services;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.Services
{
    /// <summary>
    /// Detect SEO duplicates based on  store, catalog, categories relationships and structure knowledge
    /// </summary>
    public class CatalogSeoDublicatesDetector : ISeoDuplicatesDetector
    {
        private readonly IItemService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;

        public CatalogSeoDublicatesDetector(IItemService productService, ICategoryService categoryService, IStoreService storeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _storeService = storeService;
        }

        #region ISeoConflictDetector Members
        public Task<IEnumerable<SeoInfo>> DetectSeoDuplicatesAsync(TenantIdentity tenantIdentity)
        {
            //TODO:
            throw new NotImplementedException();
            //var retVal = new List<SeoInfo>();
            //string catalogId = null;
            //if (objectType.EqualsInvariant(typeof(Store).Name))
            //{
            //    var store = await _storeService.GetByIdAsync(objectId);
            //    if (store != null)
            //    {
            //        catalogId = store.Catalog;
            //    }
            //}
            //else if (objectType.EqualsInvariant(typeof(Category).Name))
            //{
            //    var category = (await _categoryService.GetByIdsAsync(new[] { objectId }, CategoryResponseGroup.Info.ToString())).FirstOrDefault();
            //    if (category != null)
            //    {
            //        catalogId = category.CatalogId;
            //    }
            //}
            //else if (objectType.EqualsInvariant(typeof(CatalogProduct).Name))
            //{
            //    var product = (await _productService.GetByIdsAsync(new[] { objectId }, ItemResponseGroup.ItemInfo.ToString())).FirstOrDefault();
            //    if (product != null)
            //    {
            //        catalogId = product.CatalogId;
            //    }
            //}

            //if (!string.IsNullOrEmpty(catalogId))
            //{
            //    //Get all SEO owners witch related to requested catalog and contains Seo duplicates
            //    var objectsWithSeoDuplicates = await GetSeoOwnersContainsDuplicatesAsync(catalogId, allSeoDuplicates);
            //    //Need select for each seo owner one seo defined for requested container or without it if not exist
            //    var seoInfos = objectsWithSeoDuplicates.Select(x => x.SeoInfos.Where(s => s.StoreId == objectId || string.IsNullOrEmpty(s.StoreId)).OrderByDescending(s => s.StoreId).FirstOrDefault())
            //                                           .Where(x => x != null);
            //    //return only Seo infos with have duplicate slug keyword
            //    retVal = seoInfos.GroupBy(x => x.SemanticUrl).Where(x => x.Count() > 1).SelectMany(x => x).ToList();
            //}
            //return retVal;
        }
        #endregion
        /// <summary>
        /// Detect SEO duplicates  for object belongs to catalog  (physical or virtual) based on links information
        /// </summary>
        /// <param name="catalogId"></param>
        /// <param name="allDublicatedSeos"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ISeoSupport>> GetSeoOwnersContainsDuplicatesAsync(string catalogId, IEnumerable<SeoInfo> allDublicatedSeos)
        {
            var productsSeo = allDublicatedSeos.Where(x => x.ObjectType.EqualsInvariant(typeof(CatalogProduct).Name));
            var categoriesSeo = allDublicatedSeos.Where(x => x.ObjectType.EqualsInvariant(typeof(Category).Name));

            var products = await _productService.GetByIdsAsync(productsSeo.Select(x => x.ObjectId).Distinct().ToArray(), (ItemResponseGroup.Outlines | ItemResponseGroup.Seo).ToString(), catalogId);
            var categories = await _categoryService.GetByIdsAsync(categoriesSeo.Select(x => x.ObjectId).Distinct().ToArray(), (CategoryResponseGroup.WithOutlines | CategoryResponseGroup.WithSeo).ToString(), catalogId);

            var retVal = new List<ISeoSupport>();
            //Here we try to find between SEO duplicates records for products with directly or indirectly (virtual) related to requested catalog
            foreach (var product in products)
            {
                if (product.CatalogId == catalogId || product.Outlines.SelectMany(x => x.Items).Any(x => x.Id == catalogId))
                {
                    foreach (var productSeo in product.SeoInfos)
                    {
                        productSeo.Name = string.Format("{0} ({1})", product.Name, product.Code);
                    }
                    retVal.Add(product);
                }
            }
            //Here we try to find between SEO duplicates records for categories with directly or indirectly related to requested catalog
            foreach (var category in categories)
            {
                if (category.CatalogId == catalogId || category.Outlines.SelectMany(x => x.Items).Any(x => x.Id == catalogId))
                {
                    foreach (var categorySeo in category.SeoInfos)
                    {
                        categorySeo.Name = string.Format("{0}", category.Name);
                    }
                    retVal.Add(category);
                }
            }
            return retVal;
        }
    }
}
