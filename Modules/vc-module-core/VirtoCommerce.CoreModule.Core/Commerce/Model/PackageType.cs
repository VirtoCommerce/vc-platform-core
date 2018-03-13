using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.Domain.Commerce.Model
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
