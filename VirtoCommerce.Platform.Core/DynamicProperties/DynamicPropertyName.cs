using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Platform.Core.DynamicProperties
{
    public class DynamicPropertyName : ValueObject, ICloneable
    {
        /// <summary>
        /// Language ID, e.g. en-US.
        /// </summary>
        public string Locale { get; set; }
        public string Name { get; set; }


        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        } 
        #endregion
    }
}
