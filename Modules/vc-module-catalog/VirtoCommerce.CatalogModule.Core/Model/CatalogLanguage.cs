
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class CatalogLanguage : Entity
    {
        public string CatalogId { get; set; }
        public CatalogModule.Core.Model.Catalog Catalog { get; set; }

        public bool IsDefault { get; set; }
        public string LanguageCode { get; set; }
    }
}
