using VirtoCommerce.Platform.Core.Assets;
using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class AssociationConverter
    {
        public static webModel.ProductAssociation ToWebModel(this moduleModel.ProductAssociation association, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = new webModel.ProductAssociation
            {
                AssociatedObjectId = association.AssociatedObjectId,
                AssociatedObjectType = association.AssociatedObjectType,
                Quantity = association.Quantity,
                Tags = association.Tags,
                Type = association.Type,
                Priority = association.Priority
            };

            if (association.AssociatedObject != null)
            {
                var product = association.AssociatedObject as moduleModel.CatalogProduct;
                var category = association.AssociatedObject as moduleModel.Category;
                if (product != null)
                {
                    var associatedProduct = product.ToWebModel(blobUrlResolver);
                    retVal.AssociatedObjectImg = associatedProduct.ImgSrc;
                    retVal.AssociatedObjectName = associatedProduct.Name;
                }
                if (category != null)
                {
                    var associatedCategory = category.ToWebModel(blobUrlResolver);
                    retVal.AssociatedObjectImg = associatedCategory.ImgSrc;
                    retVal.AssociatedObjectName = associatedCategory.Name;
                }
            }

            return retVal;
        }

        public static moduleModel.ProductAssociation ToCoreModel(this webModel.ProductAssociation association)
        {
            return new moduleModel.ProductAssociation
            {
                AssociatedObjectId = association.AssociatedObjectId,
                AssociatedObjectType = association.AssociatedObjectType,
                Quantity = association.Quantity,
                Tags = association.Tags,
                Type = association.Type,
                Priority = association.Priority
            };
        }
    }
}
