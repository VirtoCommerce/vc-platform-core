using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class ContentFolderConverter
    {
        public static webModel.DynamicContentFolder ToWebModel(this coreModel.DynamicContentFolder folder)
        {
            var retVal = new webModel.DynamicContentFolder
            {
                Id = folder.Id,
                CreatedDate = folder.CreatedDate,
                CreatedBy = folder.CreatedBy,
                ModifiedDate = folder.ModifiedDate,
                ModifiedBy = folder.ModifiedBy,
                Description = folder.Description,
                Path = folder.Path,
                Name = folder.Name,
                Outline = folder.Outline,
                ParentFolderId = folder.ParentFolderId
            };

            return retVal;
        }

        public static coreModel.DynamicContentFolder ToCoreModel(this webModel.DynamicContentFolder folder)
        {
            var retVal = new coreModel.DynamicContentFolder
            {
                Id = folder.Id,
                CreatedDate = folder.CreatedDate,
                CreatedBy = folder.CreatedBy,
                ModifiedDate = folder.ModifiedDate,
                ModifiedBy = folder.ModifiedBy,
                Description = folder.Description,
                Name = folder.Name,
                ParentFolderId = folder.ParentFolderId
            };

            return retVal;
        }

    }
}
