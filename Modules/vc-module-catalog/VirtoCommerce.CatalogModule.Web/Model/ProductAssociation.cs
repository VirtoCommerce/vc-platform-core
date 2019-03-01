namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Class containing associated product information like 'Accessory', 'Related Item', etc.
    /// </summary>
    public class ProductAssociation
    {
        /// <summary>
        /// Gets or sets the ProductAssociation type.
        /// </summary>
        /// <value>
        /// Accessories, Up-Sales, Cross-Sales, Related etc
        /// </value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the order in which the associated product is displayed.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; set; }
        /// <summary>
        /// Gets or sets the quantity for associated object
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// Each link element can have an associated object like Product, Category, etc.
        /// Is a primary key of associated object
        /// </summary>
        public string AssociatedObjectId { get; set; }
        /// <summary>
        /// Display name for associated object
        /// </summary>
        public string AssociatedObjectName { get; set; }
        /// <summary>
        /// Associated object type
        /// </summary>
        public string AssociatedObjectType { get; set; }
        /// <summary>
        /// Associated object image URL
        /// </summary>
        public string AssociatedObjectImg  { get; set; }
        /// <summary>
        /// Association tags
        /// </summary>
        public string[] Tags { get; set; }

    }
}
