using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class LinkConverter
    {
        public static webModel.CategoryLink ToWebModel(this moduleModel.CategoryLink link)
        {
            return new webModel.CategoryLink
            {
                CatalogId = link.CatalogId,
                CategoryId = link.CategoryId,
                Priority = link.Priority
            };
        }

        public static moduleModel.CategoryLink ToCoreModel(this webModel.CategoryLink link)
        {
            return new moduleModel.CategoryLink
            {
                CatalogId = link.CatalogId,
                CategoryId = link.CategoryId,
                Priority = link.Priority
            };
        }
    }
}
