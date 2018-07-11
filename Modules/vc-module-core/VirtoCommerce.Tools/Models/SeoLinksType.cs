namespace VirtoCommerce.Tools.Models
{
    /// <summary>
    /// Determines how to generate links for products and categories.
    /// </summary>
    public enum SeoLinksType
    {
        /// <summary>
        /// /category/123
        /// /product/123
        /// </summary>
        None,

        /// <summary>
        /// /my-category
        /// /my-product
        /// </summary>
        Short,

        /// <summary>
        /// /virtual-parent-category/physical-linked-category/my-category
        /// /virtual-parent-category/physical-linked-category/my-category/my-product
        /// </summary>
        Long,

        /// <summary>
        /// virtual-parent-category/my-category
        /// virtual-parent-category/my-category/my-product
        /// </summary>
        Collapsed,
    }
}
