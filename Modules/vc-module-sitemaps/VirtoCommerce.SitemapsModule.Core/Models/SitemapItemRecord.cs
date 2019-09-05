using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItemRecord : ICloneable
    {
        public string Url { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string UpdateFrequency { get; set; }

        public decimal Priority { get; set; }

        public ICollection<SitemapItemAlternateLinkRecord> Alternates { get; set; } = new List<SitemapItemAlternateLinkRecord>();

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as SitemapItemRecord;

            if (Alternates != null)
            {
                result.Alternates = new ObservableCollection<SitemapItemAlternateLinkRecord>(Alternates.Select(x => x.Clone() as SitemapItemAlternateLinkRecord));
            }

            return result;
        }

        #endregion
    }
}
