using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Core.ModuleFileCollector
{
    public class StyleCollector : FileCollector, IStyleCollector
    {
        private readonly ILocalModuleCatalog _localModuleCatalog;

        protected override string ModuleName { get; set; }
        protected override string VirtualPath { get; set; }

        public StyleCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog)
            : base(fileVersionProvider)
        {
            _localModuleCatalog = localModuleCatalog;
        }
        internal override InternalModuleFile[] LoadModulesTargetPaths()
        {
            var result = new List<InternalModuleFile>();

            var includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().ToList();

            foreach (var includedModule in includedModules)
            {
                var stylesMetadata = includedModule.Styles.SingleOrDefault();

                if (null == stylesMetadata)
                {
                    continue;
                }

                var targetPath = Path.Join(includedModule.FullPhysicalPath, stylesMetadata.VirtualPath.Replace("/", "\\"));

                ModuleName = includedModule.ModuleName;
                VirtualPath = stylesMetadata.VirtualPath;

                result.AddRange(Handle(targetPath, "style.css", "vendor.css"));
            }

            return result.ToArray();
        }
    }
}
