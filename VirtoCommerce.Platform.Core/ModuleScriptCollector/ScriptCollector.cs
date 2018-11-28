using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.FileVersionProvider;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.ModuleScriptCollector.Normalizer;

namespace VirtoCommerce.Platform.Core.ModuleScriptCollector
{
    public class ScriptCollector : IScriptCollector
    {
        private readonly IFileVersionProvider _fileVersionProvider;
        private readonly ILocalModuleCatalog _localModuleCatalog;
        private readonly IModuleScriptPathNormalizerFactory _moduleScriptPathNormalizerFactory;

        private readonly ICollection<InternalModuleScript> _moduleScripts = new List<InternalModuleScript>();

        public ScriptCollector(IFileVersionProvider fileVersionProvider, ILocalModuleCatalog localModuleCatalog, IModuleScriptPathNormalizerFactory moduleScriptPathNormalizerFactory)
        {
            _fileVersionProvider = fileVersionProvider;
            _localModuleCatalog = localModuleCatalog;
            _moduleScriptPathNormalizerFactory = moduleScriptPathNormalizerFactory;
        }

        public ModuleScript[] Collect(bool isNeedVersionAppend)
        {
            LoadModulesTargetPaths();

            AddFileVersion(isNeedVersionAppend);

            return _moduleScripts.OfType<ModuleScript>().ToArray();
        }

        private void LoadModulesTargetPaths()
        {
            var includedModules = _localModuleCatalog.Modules.OfType<ManifestModuleInfo>().ToList();

            foreach (var includedModule in includedModules)
            {
                var scriptsMetadata = includedModule.Scripts.SingleOrDefault();

                if (null == scriptsMetadata)
                {
                    continue;
                }

                var scriptsFolderName = scriptsMetadata.VirtualPath.Split("/").Last();

                var targetPath = Path.Join(includedModule.FullPhysicalPath, scriptsFolderName, "dist");

                if (Directory.Exists(targetPath))
                {
                    var moduleScript = Path.Join(targetPath, "app.js");
                    var moduleVendor = Path.Join(targetPath, "vendor.js");

                    var normalizer = _moduleScriptPathNormalizerFactory.Create(scriptsMetadata.VirtualPath, includedModule.ModuleName, includedModule.Assembly.GetName().Name);

                    if (File.Exists(moduleScript))
                    {
                        _moduleScripts.Add(new InternalModuleScript
                        {
                            Path = moduleScript,
                            WebPath = normalizer.Normalize(moduleScript)
                        });
                    }

                    if (File.Exists(moduleVendor))
                    {
                        _moduleScripts.Add(new InternalModuleScript
                        {
                            Path = moduleVendor,
                            WebPath = normalizer.Normalize(moduleScript),
                            IsVendor = true
                        });
                    }
                }
            }
        }

        private void AddFileVersion(bool isNeedAppendVersion)
        {
            if (isNeedAppendVersion)
            {
                foreach (var moduleScript in _moduleScripts)
                {
                    moduleScript.Version = _fileVersionProvider.GetFileVersion(moduleScript.Path);
                }
            }
        }
    }
}
