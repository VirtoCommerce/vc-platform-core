using System.Linq;
using VirtoCommerce.CatalogModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class ExportableCatalog : Catalog, IExportable, IExportViewable
    {
        new public string Name { get; set; }
        public string Code { get; set; }
        public string ImageUrl { get; set; }
        public string Parent { get; set; }
        public string Type { get; set; }

        public virtual ExportableCatalog FromModel(Catalog source)
        {
            Type = nameof(Catalog);
            Name = source.Name;
            Parent = source.OuterId;
            Id = source.Id;
            OuterId = source.OuterId;
            IsVirtual = source.IsVirtual;
            Languages = source.Languages?.Select(x => x.Clone()).OfType<CatalogLanguage>().ToList(); ;
            return this;
        }
    }
}
