using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class ProductAssociation : ValueObject, ICloneable
    {
        /// <summary>
        /// Association type (Accessories, Up-Sales, Cross-Sales, Related etc)
        /// </summary>
        public string Type { get; set; }

        public int Priority { get; set; }

        public int? Quantity { get; set; }
        /// <summary>
        /// Each link element can have an associated object like Product, Category, etc.
        /// Is a primary key of associated object
        /// </summary>
        public string AssociatedObjectId { get; set; }
        /// <summary>
        /// Associated object type : 'product', 'category' etc
        /// </summary>
        public string AssociatedObjectType { get; set; }
        /// <summary>
        /// Associated object
        /// </summary>
        public IEntity AssociatedObject { get; set; }

        public string[] Tags { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
