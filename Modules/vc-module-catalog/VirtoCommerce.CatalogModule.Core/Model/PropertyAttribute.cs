
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
	public class PropertyAttribute : AuditableEntity
    {
		public string PropertyId { get; set; }
        public Property Property { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }
}