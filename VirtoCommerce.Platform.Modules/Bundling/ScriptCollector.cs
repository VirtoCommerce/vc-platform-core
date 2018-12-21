using System.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class ScriptCollector : FileCollector, IScriptCollector
    {
        private readonly ILocalModuleCatalog _localModuleCatalog;

        public ScriptCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog)
            : base(fileVersionProvider)
        {
            _localModuleCatalog = localModuleCatalog;
        }

        protected override BundleMetadata GetMetadata()
        {
            var includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().ToList();

            return new BundleMetadata
            {
                BundleName = "app.js",
                VendorName = "vendor.js",
                ModulesMetadata = includedModules.Select(m => new ModuleMetadata
                {
                    FullPhysicalModulePath = m.FullPhysicalPath,
                    ModuleName = m.ModuleName,
                    VirtualPath = m.Scripts?.VirtualPath
                }).ToArray()
            };
        }
    }
}
