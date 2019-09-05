using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItem : AuditableEntity, ICloneable
    {
        public string SitemapId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public string ObjectId { get; set; }

        public string ObjectType { get; set; }

        public string UrlTemplate { get; set; }

        public ICollection<SitemapItemRecord> ItemsRecords { get; set; } = new List<SitemapItemRecord>();

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as SitemapItem;

            if (ItemsRecords != null)
            {
                result.ItemsRecords = new ObservableCollection<SitemapItemRecord>(ItemsRecords.Select(x => x.Clone() as SitemapItemRecord));
            }

            return result;
        }

        #endregion

    }
}
