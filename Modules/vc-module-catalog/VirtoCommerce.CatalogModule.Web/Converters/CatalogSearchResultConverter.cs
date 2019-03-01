using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Assets;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class CatalogSearchResultConverter
    {
        public static webModel.CatalogSearchResult ToWebModel(this coreModel.Search.SearchResult result, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = new webModel.CatalogSearchResult
            {
                ProductsTotalCount = result.ProductsTotalCount
            };

            if (result.Products != null)
            {
                //Parallel conversation for better performance
                var preservedOrder = result.Products.Select(x => x.Id).ToList();
                var productDtos = new ConcurrentBag<CatalogModule.Web.Model.Product>();
                Parallel.ForEach(result.Products, (x) =>
                {
                    productDtos.Add(x.ToWebModel(blobUrlResolver));
                });
                retVal.Products = productDtos.OrderBy(i => preservedOrder.IndexOf(i.Id)).ToArray();
            }

            if (result.Categories != null)
            {
                var preservedOrder = result.Categories.Select(x => x.Id).ToList();
                var categoryDtos = new ConcurrentBag<CatalogModule.Web.Model.Category>();
                Parallel.ForEach(result.Categories, (x) =>
                {
                    categoryDtos.Add(x.ToWebModel(blobUrlResolver));
                });
                retVal.Categories = categoryDtos.OrderBy(i => preservedOrder.IndexOf(i.Id)).ToArray();
            }

            if (result.Aggregations != null)
            {
                retVal.Aggregations = result.Aggregations.Select(a => a.ToWebModel()).ToArray();
            }

            return retVal;
        }
    }
}
