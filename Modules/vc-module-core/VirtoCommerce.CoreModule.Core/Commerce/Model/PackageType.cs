using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CoreModule.Core.Commerce.Model
{
    /// <summary>
    /// Represent predefined dimensions package type
    /// </summary>
    public class PackageType : Entity
    {
        /// <summary>
        /// Package type name
        /// </summary>
        public string Name { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string MeasureUnit { get; set; }

    }
}
