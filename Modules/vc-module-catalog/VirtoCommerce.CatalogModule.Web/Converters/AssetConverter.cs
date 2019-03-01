using System.Web;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using coreModel = VirtoCommerce.CatalogModule.Core.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
    public static class AssetConverter
    {
        public static webModel.Image ToWebModel(this coreModel.Image image, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = new webModel.Image
            {
                Group = image.Group,
                Id = image.Id,
                LanguageCode = image.LanguageCode,
                Name = image.Name,
                IsInherited = image.IsInherited,
                SortOrder = image.SortOrder
            };
            //Do not use omu.InjectFrom for performance reasons 

            if (blobUrlResolver != null)
            {
                retVal.Url = blobUrlResolver.GetAbsoluteUrl(image.Url);
            }
            retVal.RelativeUrl = image.Url;
            return retVal;
        }

        public static webModel.Asset ToWebModel(this coreModel.Asset asset, IBlobUrlResolver blobUrlResolver)
        {
            var retVal = new webModel.Asset
            {
                Size = asset.Size,
            };

            if (asset.Name == null)
            {
                retVal.Name = HttpUtility.UrlDecode(System.IO.Path.GetFileName(asset.Url));
            }
            if (asset.MimeType == null)
            {
                retVal.MimeType = MimeTypeResolver.ResolveContentType(asset.Name);
            }
            if (blobUrlResolver != null)
            {
                retVal.Url = blobUrlResolver.GetAbsoluteUrl(asset.Url);
            }
            retVal.RelativeUrl = asset.Url;
            retVal.ReadableSize = retVal.Size.ToHumanReadableSize();
            return retVal;
        }

        public static coreModel.Image ToCoreModel(this webModel.Image image)
        {
            return new coreModel.Image
            {
                Group = image.Group,
                Id = image.Id,
                LanguageCode = image.LanguageCode,
                Name = image.Name,
                IsInherited = image.IsInherited,
                SortOrder = image.SortOrder,
                Url = image.RelativeUrl
            };
        }

        public static coreModel.Asset ToCoreModel(this webModel.Asset asset)
        {
            return new coreModel.Asset
            {
                Size = asset.Size,
                Url = asset.RelativeUrl,
                MimeType = asset.MimeType,
                Group = asset.Group,
            };
        }
    }
}
