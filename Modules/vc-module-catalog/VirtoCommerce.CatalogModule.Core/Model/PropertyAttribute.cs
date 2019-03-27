
using System;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyAttribute : AuditableEntity, ICloneable
    {
        public string PropertyId { get; set; }
        [JsonIgnore]
        public Property Property { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }

        #region ICloneable members
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
