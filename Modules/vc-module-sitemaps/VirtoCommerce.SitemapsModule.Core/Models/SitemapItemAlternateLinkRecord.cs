using System;

namespace VirtoCommerce.SitemapsModule.Core.Models
{
    public class SitemapItemAlternateLinkRecord : ICloneable
    {
        public string Url { get; set; }

        public string Language { get; set; }

        public string Type { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as SitemapItemAlternateLinkRecord;
        }

        #endregion
    }
}
