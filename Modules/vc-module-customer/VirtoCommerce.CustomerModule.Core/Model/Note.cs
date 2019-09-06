using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CustomerModule.Core.Model
{
    public class Note : AuditableEntity, ICloneable
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            return MemberwiseClone() as Note;
        }

        #endregion
    }
}
