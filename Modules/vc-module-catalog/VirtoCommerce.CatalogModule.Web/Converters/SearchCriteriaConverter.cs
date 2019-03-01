using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class SearchCriteriaConverter
    {
        public static coreModel.Search.SearchCriteria ToCoreModel(this webModel.SearchCriteria criteria)
        {
            var retVal = new coreModel.Search.SearchCriteria
            {
                Keyword = criteria.Keyword,
                Code = criteria.Code,
                Sort = criteria.Sort,
                Take = criteria.Take,
                Skip = criteria.Skip,
                Currency = criteria.Currency,
                SearchInChildren = criteria.SearchInChildren,
                SearchInVariations = criteria.SearchInVariations,
                CategoryId = criteria.CategoryId,
                CatalogId = criteria.CatalogId,
                StoreId = criteria.StoreId,
                LanguageCode = criteria.LanguageCode,
                ResponseGroup = criteria.ResponseGroup,
                CategoryIds = criteria.CategoryIds,
                CatalogIds = criteria.CatalogIds,
                PricelistIds = criteria.PricelistIds,
                StartPrice = criteria.StartPrice,
                EndPrice = criteria.EndPrice,
                IndexDate = criteria.IndexDate,
                PricelistId = criteria.PricelistId,
                Terms = criteria.Terms,
                Facets = criteria.Facets,
                Outline = criteria.Outline,
                OnlyBuyable = criteria.OnlyBuyable,
                OnlyWithTrackingInventory = criteria.OnlyWithTrackingInventory,
                ProductType = criteria.ProductType,
                ProductTypes = criteria.ProductTypes,
                VendorId = criteria.VendorId,
                VendorIds = criteria.VendorIds,
                StartDateFrom = criteria.StartDateFrom
            };

            if (!criteria.PropertyValues.IsNullOrEmpty())
            {
                retVal.PropertyValues = criteria.PropertyValues.Select(x => x.ToCoreModel()).ToArray();
            }

            return retVal;

        }
    }
}
