using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using moduleModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class CategoryConverter
    {
        public static webModel.Category ToWebModel(this moduleModel.Category category, IBlobUrlResolver blobUrlResolver = null, bool convertProps = true)
        {
            var retVal = new webModel.Category
            {
                Id = category.Id,
                IsActive = category.IsActive,
                IsVirtual = category.IsVirtual,
                Name = category.Name,
                ParentId = category.ParentId,
                Path = category.Path,
                TaxType = category.TaxType,
                CatalogId = category.CatalogId,
                Code = category.Code,
                CreatedBy = category.CreatedBy,
                CreatedDate = category.CreatedDate,
                ModifiedBy = category.ModifiedBy,
                ModifiedDate = category.ModifiedDate,
                SeoInfos = category.SeoInfos
            };

            //Do not use omu.InjectFrom for performance reasons 

            if (!category.Outlines.IsNullOrEmpty())
            {
                //Minimize outline size
                retVal.Outlines = category.Outlines.Select(x => x.ToWebModel()).ToList();
            }

            //Init outline and path
            if (category.Parents != null)
            {
                retVal.Outline = string.Join("/", category.Parents.Select(x => x.Id));
                retVal.Path = string.Join("/", category.Parents.Select(x => x.Name));
            }

            //For virtual category links not needed
            if (!category.IsVirtual && category.Links != null)
            {
                retVal.Links = category.Links.Select(x => x.ToWebModel()).ToList();
            }

            //Need add property for each meta info
            retVal.Properties = new List<webModel.Property>();
            if (convertProps)
            {
                if (!category.Properties.IsNullOrEmpty())
                {
                    foreach (var property in category.Properties)
                    {
                        var webModelProperty = property.ToWebModel();
                        webModelProperty.Values = new List<webModel.PropertyValue>();
                        webModelProperty.IsManageable = true;
                        webModelProperty.IsReadOnly = property.Type != moduleModel.PropertyType.Category;
                        retVal.Properties.Add(webModelProperty);
                    }
                }

                //Populate property values
                if (category.PropertyValues != null)
                {
                    var sort = false;
                    foreach (var propValue in category.PropertyValues.Select(x => x.ToWebModel()))
                    {
                        var property = retVal.Properties.FirstOrDefault(x => x.Id == propValue.PropertyId);
                        if (property == null)
                        {
                            property = retVal.Properties.FirstOrDefault(x => x.Name.EqualsInvariant(propValue.PropertyName));
                        }
                        if (property == null)
                        {
                            //Need add dummy property for each value without property
                            property = new webModel.Property(propValue, category.CatalogId, moduleModel.PropertyType.Category);
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

            if (category.Images != null)
            {
                retVal.Images = category.Images.Select(x => x.ToWebModel(blobUrlResolver)).ToList();
            }

            return retVal;
        }

        public static moduleModel.Category ToModuleModel(this webModel.Category category)
        {
            var retVal = new moduleModel.Category
            {
                Id = category.Id,
                IsActive = category.IsActive,
                IsVirtual = category.IsVirtual,
                Name = category.Name,
                ParentId = category.ParentId,
                Path = category.Path,
                TaxType = category.TaxType,
                CatalogId = category.CatalogId,
                Code = category.Code,
                CreatedBy = category.CreatedBy,
                CreatedDate = category.CreatedDate,
                ModifiedBy = category.ModifiedBy,
                ModifiedDate = category.ModifiedDate,
                SeoInfos = category.SeoInfos
            };

            if (category.Links != null)
            {
                retVal.Links = category.Links.Select(x => x.ToCoreModel()).ToList();
            }

            if (category.Properties != null)
            {
                retVal.PropertyValues = new List<moduleModel.PropertyValue>();
                foreach (var property in category.Properties)
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

            if (category.Images != null)
            {
                retVal.Images = category.Images.Select(x => x.ToCoreModel()).ToList();
            }

            return retVal;
        }
    }
}
