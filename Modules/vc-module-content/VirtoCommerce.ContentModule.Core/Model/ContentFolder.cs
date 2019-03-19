using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    /// <summary>
    /// Represent content folder
    /// </summary>
    public class ContentFolder : ContentItem
    {
        public ContentFolder()
            : base("folder")
        {

        }

        public ContentFolder ToContentModel (BlobFolder blobFolder)
        {
            var result = AbstractTypeFactory<ContentFolder>.TryCreateInstance();



            return result;

        }

        public BlobFolder ToBlobModel( BlobFolder blobFolder)
        {
            blobFolder.Name = Name;
            blobFolder.Url = Url;
            blobFolder.ParentUrl = ParentUrl;
            blobFolder.RelativeUrl = RelativeUrl;

            return blobFolder;
        }
    }
}
