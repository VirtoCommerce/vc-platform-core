using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class ProductAssociation : ValueObject, IHasOuterId
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
        [JsonIgnore]
        public Entity AssociatedObject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OuterId { get; set; }

        /// <summary>
        /// Display name for associated object
        /// </summary>
        public virtual string AssociatedObjectName => (AssociatedObject is IHasName hasName) ? hasName.Name : null;
        /// <summary>
        /// Associated object image URL
        /// </summary>
        public virtual string AssociatedObjectImg => (AssociatedObject is IHasImages hasImages) ? hasImages.ImgSrc : null;

        public string[] Tags { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return AssociatedObjectId;
            yield return AssociatedObjectType;
            yield return Type;
        }

    }
}
