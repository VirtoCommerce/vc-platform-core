using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Web.Model
{
    public class PropertyValidationRule : Entity
    {
        public bool IsUnique { get; set; }

        public int? CharCountMin { get; set; }

        public int? CharCountMax { get; set; }

        public string RegExp { get; set; }
    }
}
