using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Package
{
    /// <summary>
    /// Represent predefined dimensions package type
    /// </summary>
    public class PackageType : Entity, ICloneable
    {
        /// <summary>
        /// Package type name
        /// </summary>
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string MeasureUnit { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as PackageType;
            return result;
        }

        #endregion

    }
}
