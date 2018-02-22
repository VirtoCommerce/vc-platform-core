
using VirtoCommerce.Platform.Core.Common;
namespace VirtoCommerce.Domain.Catalog.Model
{
	public class CategoryLink : ValueObject
    {
        /// <summary>
        /// Product order position in virtual catalog
        /// </summary>
        public int Priority { get; set; }
		public string CatalogId { get; set; }
		public Catalog Catalog { get; set; }
        public string CategoryId { get; set; }
		public Category Category { get; set; }
    }
}
