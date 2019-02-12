using coreModel = VirtoCommerce.MarketingModule.Core.Model;
using webModel = VirtoCommerce.MarketingModule.Web.Model;

namespace VirtoCommerce.MarketingModule.Web.Converters
{
    public static class DynamicContentPlaceConverter
    {
        public static webModel.DynamicContentPlace ToWebModel(this coreModel.DynamicContentPlace place)
        {
            var retVal = new webModel.DynamicContentPlace
            {
                Id = place.Id,
                CreatedDate = place.CreatedDate,
                CreatedBy = place.CreatedBy,
                ModifiedDate = place.ModifiedDate,
                ModifiedBy = place.ModifiedBy,
                Description = place.Description,
                Name = place.Name,
                FolderId = place.FolderId,
                ImageUrl = place.ImageUrl,
            };

            if (place.Folder != null)
            {
                retVal.Outline = place.Folder.Outline;
                retVal.Path = place.Folder.Path;
            }
            return retVal;
        }

        public static coreModel.DynamicContentPlace ToCoreModel(this webModel.DynamicContentPlace place)
        {
            var retVal = new coreModel.DynamicContentPlace
            {
                Id = place.Id,
                CreatedDate = place.CreatedDate,
                CreatedBy = place.CreatedBy,
                ModifiedDate = place.ModifiedDate,
                ModifiedBy = place.ModifiedBy,
                Description = place.Description,
                Name = place.Name,
                FolderId = place.FolderId,
                ImageUrl = place.ImageUrl,
            };
            return retVal;
        }

    }
}
