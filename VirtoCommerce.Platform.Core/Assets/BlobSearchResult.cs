using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.Assets
{
    public class BlobSearchResult 
    {
        public BlobSearchResult()
        {
            Folders = new List<BlobFolder>();
            Items = new List<BlobInfo>();
        }
        public ICollection<BlobFolder> Folders { get; set; }
        public ICollection<BlobInfo> Items { get; set; }

        public List<AssetListItem> Result
        {
            get
            {
                var result =  new List<AssetListItem>();

                result.AddRange(Folders.Select( f => new AssetListItem
                {
                    Type = f.Type,
                    Name = f.Name,
                    Url = f.Url,
                    ParentUrl = f.ParentUrl,
                    RelativeUrl = f.RelativeUrl

                }));
                result.AddRange(Items.Select( i => new AssetListItem
                {
                    Type = i.Type,
                    Name = i.Name,
                    RelativeUrl = i.RelativeUrl,
                    ParentUrl = i.RelativeUrl,
                    Url = i.Url,
                    ContentType = i.ContentType,
                    ModifiedDate = i.ModifiedDate
                }));

                return result;
            }
        }

    }
}
