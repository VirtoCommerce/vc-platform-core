using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;
using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class CatalogConverter
    {
        public static webModel.Catalog ToWebModel(this moduleModel.Catalog catalog, bool convertProps = true)
        {
            var retVal = new webModel.Catalog
            {
                Id = catalog.Id,
                Name = catalog.Name,
                IsVirtual = catalog.IsVirtual,
                Properties = new List<webModel.Property>()
            };

            if (catalog.Languages != null)
            {
                retVal.Languages = catalog.Languages.Select(x => x.ToWebModel()).ToList();
            }

            if (convertProps)
            {
                //Need add property for each meta info
                if (catalog.Properties != null)
                {
                    foreach (var property in catalog.Properties)
                    {
                        var webModelProperty = property.ToWebModel();
                        webModelProperty.Values = new List<webModel.PropertyValue>();
                        webModelProperty.IsManageable = true;
                        webModelProperty.IsReadOnly = property.Type != moduleModel.PropertyType.Catalog;
                        retVal.Properties.Add(webModelProperty);
                    }
                }

                //Populate property for property values
                if (catalog.PropertyValues != null)
                {
                    var sort = false;
                    foreach (var propValue in catalog.PropertyValues.Select(x => x.ToWebModel()))
                    {
                        var property = retVal.Properties.FirstOrDefault(x => x.Id == propValue.PropertyId);
                        if (property == null)
                        {
                            property = retVal.Properties.FirstOrDefault(x => x.Name.EqualsInvariant(propValue.PropertyName));
                        }
                        if (property == null)
                        {
                            //Need add dummy property for each value without property
                            property = new webModel.Property(propValue, catalog.Id, moduleModel.PropertyType.Catalog);
                            retVal.Properties.Add(property);
                            sort = true;
                        }
                        property.Values.Add(propValue);
                    }
                    if (sort)
                    {
                        retVal.Properties = retVal.Properties.OrderBy(x => x.Name).ToList();
                    }
                }
            }
            return retVal;
        }

        public static moduleModel.Catalog ToCoreModel(this webModel.Catalog catalog)
        {
            var retVal = new moduleModel.Catalog
            {
                Id = catalog.Id,
                Name = catalog.Name,
                IsVirtual = catalog.IsVirtual,
            };

            if (catalog.Languages != null)
            {
                retVal.Languages = catalog.Languages.Select(x => x.ToCoreModel()).ToList();
            }

            if (catalog.Properties != null)
            {
                retVal.PropertyValues = new List<moduleModel.PropertyValue>();
                foreach (var property in catalog.Properties)
                {
                    foreach (var propValue in property.Values ?? Enumerable.Empty<webModel.PropertyValue>())
                    {
                        propValue.ValueType = property.ValueType;
                        //Need populate required fields
                        propValue.PropertyId = property.Id;
                        propValue.PropertyName = property.Name;
                        retVal.PropertyValues.Add(propValue.ToCoreModel());
                    }
                }

            }
            return retVal;
        }
    }
}
