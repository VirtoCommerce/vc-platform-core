using VirtoCommerce.Platform.Core.DynamicProperties;
using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class ContentItemConverter
    {
        public static webModel.DynamicContentItem ToWebModel(this coreModel.DynamicContentItem content)
        {
            var retVal = new webModel.DynamicContentItem();
            //TODO
            if (content.Folder != null)
            {
                retVal.Outline = content.Folder.Outline;
                retVal.Path = content.Folder.Path;
            }
            retVal.DynamicProperties = content.DynamicProperties;
            return retVal;
        }

        public static coreModel.DynamicContentItem ToCoreModel(this webModel.DynamicContentItem content)
        {
            var retVal = new coreModel.DynamicContentItem();
            //TODO
            retVal.DynamicProperties = content.DynamicProperties;
            if (content.DynamicProperties != null)
            {
                retVal.ContentType = retVal.GetDynamicPropertyValue<string>("Content type", null);
            }
            return retVal;
        }

    }
}
