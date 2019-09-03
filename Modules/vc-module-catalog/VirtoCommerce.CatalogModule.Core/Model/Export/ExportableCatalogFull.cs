using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Core.Model.Export
{
    public class ExportableCatalogFull : IExportable
    {
        public string Id { get; set; }

        public object Clone()
        {
            var result = MemberwiseClone() as ExportableCatalogFull;
            result.Catalogs = Catalogs?.Select(x => x.Clone() as Catalog).ToList();
            result.Categories = Categories?.Select(x => x.Clone() as Category).ToList();
            result.Properties = Properties?.Select(x => x.Clone() as Property).ToList();
            result.PropertyDictionaryItems = PropertyDictionaryItems?.Select(x => x.Clone() as PropertyDictionaryItem).ToList();
            result.CatalogProducts = CatalogProducts?.Select(x => x.Clone() as CatalogProduct).ToList();
            return result;
        }

        public ICollection<Catalog> Catalogs { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<PropertyDictionaryItem> PropertyDictionaryItems { get; set; }
        public ICollection<CatalogProduct> CatalogProducts { get; set; }
    }
}
