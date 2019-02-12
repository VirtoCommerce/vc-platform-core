using VirtoCommerce.Platform.Core.DynamicProperties;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class ContentItemConverter
    {
        public static webModel.DynamicContentItem ToWebModel(this coreModel.DynamicContentItem content)
        {
            var retVal = new webModel.DynamicContentItem
            {
                Id = content.Id,
                CreatedDate = content.CreatedDate,
                CreatedBy = content.CreatedBy,
                ModifiedDate = content.ModifiedDate,
                ModifiedBy = content.ModifiedBy,
                Description = content.Description,
                Name = content.Name,
                ContentType = content.ContentType,
                DynamicProperties = content.DynamicProperties,
                FolderId = content.FolderId,
                ImageUrl = content.ImageUrl,
            };

            if (content.Folder != null)
            {
                retVal.Outline = content.Folder.Outline;
                retVal.Path = content.Folder.Path;
            }

            return retVal;
        }

        public static coreModel.DynamicContentItem ToCoreModel(this webModel.DynamicContentItem content)
        {
            var retVal = new coreModel.DynamicContentItem
            {
                Id = content.Id,
                CreatedDate = content.CreatedDate,
                CreatedBy = content.CreatedBy,
                ModifiedDate = content.ModifiedDate,
                ModifiedBy = content.ModifiedBy,
                Description = content.Description,
                Name = content.Name,
                ContentType = content.ContentType,
                DynamicProperties = content.DynamicProperties,
                FolderId = content.FolderId,
                ImageUrl = content.ImageUrl,
            };

            if (content.DynamicProperties != null)
            {
                retVal.ContentType = retVal.GetDynamicPropertyValue<string>("Content type", null);
            }
            return retVal;
        }

    }
}
