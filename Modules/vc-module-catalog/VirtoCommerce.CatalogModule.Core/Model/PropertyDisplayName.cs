using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class PropertyDisplayName : IHasLanguage
    {
        public string Name { get; set; }
        public string LanguageCode { get; set; }

    }
}
