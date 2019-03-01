using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class PropertyAttributeConverter
    {
        public static webModel.PropertyAttribute ToWebModel(this moduleModel.PropertyAttribute attribute)
        {
            return new webModel.PropertyAttribute
            {
                Id = attribute.Id,
                Name = attribute.Name,
                Value = attribute.Value
            };
        }

        public static moduleModel.PropertyAttribute ToCoreModel(this webModel.PropertyAttribute attribute)
        {
            return new moduleModel.PropertyAttribute
            {
                Id = attribute.Id,
                Name = attribute.Name,
                Value = attribute.Value
            };
        }
    }
}
