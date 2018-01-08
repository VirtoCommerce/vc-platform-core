namespace VirtoCommerce.Platform.Modules
{
    public class LocalStorageModuleCatalogOptions
    {
        public string DiscoveryPath { get; set; }
        public string VirtualPath { get; set; }
        public string ProbingPath { get; set; }
        public string[] AssemblyFileExtensions { get; set; } = new[] { ".dll", ".pdb", ".exe", ".xml" };
    }
}
