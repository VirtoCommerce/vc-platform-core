using System.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class StyleCollector : FileCollector, IStyleCollector
    {
        private readonly ILocalModuleCatalog _localModuleCatalog;

        public StyleCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog)
            : base(fileVersionProvider)
        {
            _localModuleCatalog = localModuleCatalog;
        }

        protected override BundleMetadata GetMetadata()
        {
            var includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>();

            return new BundleMetadata
            {
                BundleName = "style.css",
                VendorName = "vendor.css",
                ModulesMetadata = includedModules.Select(m => new ModuleMetadata
                {
                    FullPhysicalModulePath = m.FullPhysicalPath,
                    ModuleName = m.ModuleName,
                    VirtualPath = m.Styles?.VirtualPath
                }).ToArray()
            };
        }
    }
}
