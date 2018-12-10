using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Extensions;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Core.ModuleFileCollector
{
    public class ScriptCollector : FileCollector, IScriptCollector
    {
        private readonly ILocalModuleCatalog _localModuleCatalog;

        protected override string ModuleName { get; set; }
        protected override string VirtualPath { get; set; }

        public ScriptCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog)
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
                var scriptsMetadata = includedModule.Scripts.SingleOrDefault();

                if (null == scriptsMetadata)
                {
                    continue;
                }

                var moduleRootFolderName = scriptsMetadata.VirtualPath.Split("/").First();
                var moduleFolderName = includedModule.Assembly.GetName().Name;

                var scriptsFolderName = scriptsMetadata.VirtualPath.GetRelativeFilePath(moduleRootFolderName, moduleFolderName);

                var targetPath = Path.Join(includedModule.FullPhysicalPath, scriptsFolderName);

                ModuleName = includedModule.ModuleName;
                VirtualPath = scriptsMetadata.VirtualPath;

                result.AddRange(Handle(targetPath, "app.js", "vendor.js"));
            }

            return result.ToArray();
        }
    }
}
