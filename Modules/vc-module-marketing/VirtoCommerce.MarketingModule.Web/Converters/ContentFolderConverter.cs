using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class ContentFolderConverter
    {
        public static webModel.DynamicContentFolder ToWebModel(this coreModel.DynamicContentFolder folder)
        {
            var retVal = new webModel.DynamicContentFolder();
            //TODO
            return retVal;
        }

        public static coreModel.DynamicContentFolder ToCoreModel(this webModel.DynamicContentFolder folder)
        {
            var retVal = new coreModel.DynamicContentFolder();
            //TODO
            return retVal;
        }

    }
}
