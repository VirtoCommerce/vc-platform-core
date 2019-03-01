namespace VirtoCommerce.CatalogModule.Web.Model
{
    /// <summary>
    /// Information to search and create links to categories and items
    /// </summary>
    public class BulkLinkCreationRequest
    {
        public SearchCriteria SearchCriteria { get; set; }

        /// <summary>
        /// The target category identifier for the link
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// The target catalog identifier for the link
        /// </summary>
        public string CatalogId { get; set; }
    }
}
