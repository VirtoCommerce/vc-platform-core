using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtoCommerce.CoreModule.Core.Outlines
{
    /// <summary>
    /// Represents the path from the catalog to one of the child objects (product or category):
    /// catalog/parent-category1/.../parent-categoryN/object
    /// </summary>
    public class Outline : ICloneable
    {
        /// <summary>
        /// Outline parts
        /// </summary>
        public ICollection<OutlineItem> Items { get; set; }

        public override string ToString()
        {
            return Items == null ? null : string.Join("/", Items);
        }

        public object Clone()
        {
            var result = MemberwiseClone() as Outline;
            result.Items = Items?.Select(x => x.Clone() as OutlineItem).ToList();
            return result;
        }
    }
}
