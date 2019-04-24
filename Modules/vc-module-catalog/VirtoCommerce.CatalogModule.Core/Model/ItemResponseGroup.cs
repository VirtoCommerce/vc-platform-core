using System;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    [Flags]
    public enum ItemResponseGroup
    {
        None = 0,
        /// <summary>
        /// Only simple product information and properties without meta information
        /// </summary>
		ItemInfo = 1,
        /// <summary>
        /// With images and assets
        /// </summary>
		ItemAssets = 1 << 1,
        WithImages = ItemAssets,
        /// <summary>
        /// With properties meta information
        /// </summary>
		ItemProperties = 1 << 2,
        WithProperties = ItemProperties,
        Properties = ItemProperties,
        /// <summary>
        /// With product associations
        /// </summary>
        ItemAssociations = 1 << 3,
        /// <summary>
        /// With descriptions
        /// </summary>
		ItemEditorialReviews = 1 << 4,
        /// <summary>
        /// With all product variations
        /// </summary>
		Variations = 1 << 5,
        WithVariations = Variations,
        /// <summary>
        /// With product SEO information
        /// </summary>
        Seo = 1 << 6,
        WithSeo = Seo,
        /// <summary>
        /// With outgoing product links to virtual catalog or categories
        /// </summary>
        Links = 1 << 7,
        WithLinks = Links,
        /// <summary>
        /// With product inventory information
        /// </summary>
        Inventory = 1 << 8,
        /// <summary>
        /// With category outlines
        /// </summary>
        Outlines = 1 << 9,
        WithOutlines = Outlines,
        /// <summary>
        /// With product referenced associations
        /// </summary>
        ReferencedAssociations = 1 << 10,

        ItemSmall = ItemInfo | ItemAssets | ItemEditorialReviews | Seo,
        ItemMedium = ItemSmall | ItemAssociations | ReferencedAssociations | ItemProperties,
        ItemLarge = ItemMedium | Variations | Links | Inventory | Outlines,
        Full = ItemLarge
    }
}
