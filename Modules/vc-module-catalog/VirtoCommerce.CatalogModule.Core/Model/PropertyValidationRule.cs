using System;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    /// <summary>
    /// Represents property validation rules definition
    /// </summary>
    public class PropertyValidationRule : Entity, ICloneable
    {
        /// <summary>
        /// Uniquie value flag constrain
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// Down chars count border or null if no defined
        /// </summary>
        public int? CharCountMin { get; set; }

        /// <summary>
        /// Upper chars count border or null if no defined
        /// </summary>
        public int? CharCountMax { get; set; }
        /// <summary>
        /// Custom regular expression
        /// </summary>
        public string RegExp { get; set; }

        public string PropertyId { get; set; }

        [JsonIgnore]
        public Property Property { get; set; }

        public object Clone()
        {
            var retVal = base.MemberwiseClone() as PropertyValidationRule;
            retVal.Property = Property != null ? Property.Clone() as Property : null;
            return retVal;
        }
    }
}
