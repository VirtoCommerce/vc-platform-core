using VirtoCommerce.CatalogModule.Core.Model;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public class TabularCategory
    {
        public string CatalogId { get; set; }
        public Catalog Catalog { get; set; }
        public string ParentId { get; set; }
        public Category Parent { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Outline { get; set; }
        public string Path { get; set; }
        public bool IsVirtual { get; set; }
        public int Level { get; set; }
        public string PackageType { get; set; }
        public int Priority { get; set; }
        public bool? IsActive { get; set; }
        public string OuterId { get; set; }
        public string TaxType { get; set; }
        public string SeoObjectType { get; set; }
        public string ImgSrc { get; set; }
        public bool IsInherited { get; set; }
    }
}
