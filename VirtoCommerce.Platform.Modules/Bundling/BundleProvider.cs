using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class BundleProvider : IBundleProvider
    {
        private readonly IFileVersionProvider _fileVersionProvider;

        protected BundleProvider(IFileVersionProvider fileVersionProvider)
        {
            _fileVersionProvider = fileVersionProvider;
        }

        public ModuleFile[] CollectScripts(IReadOnlyCollection<ManifestModuleInfo> modulesInfo, bool isNeedVersionAppend)
        {
            var modulesMetadata = modulesInfo.Select(m => new ModuleMetadata
            {
                FullPhysicalModulePath = m.FullPhysicalPath,
                ModuleName = m.ModuleName,
                VirtualPath = m.Scripts?.VirtualPath,
                FileNames = m.Scripts?.FileName
            }).ToArray();

            return LoadModulesTargetPaths(modulesMetadata, isNeedVersionAppend);
        }

        public ModuleFile[] CollectStyles(IReadOnlyCollection<ManifestModuleInfo> modulesInfo, bool isNeedVersionAppend)
        {
            var modulesMetadata = modulesInfo.Select(m => new ModuleMetadata
            {
                FullPhysicalModulePath = m.FullPhysicalPath,
                ModuleName = m.ModuleName,
                VirtualPath = m.Styles?.VirtualPath,
                FileNames = m.Styles?.FileName
            }).ToArray();

            return LoadModulesTargetPaths(modulesMetadata, isNeedVersionAppend);
        }

        protected virtual ModuleFile[] LoadModulesTargetPaths(ModuleMetadata[] modulesMetadata, bool isNeedVersionAppend)
        {
            var result = new List<ModuleFile>();

            foreach (var moduleMetadata in modulesMetadata)
            {

                if (moduleMetadata.VirtualPath == null)
                {
                    continue;
                }

                var targetPath = Path.Join(moduleMetadata.FullPhysicalModulePath, moduleMetadata.VirtualPath.Replace('/', Path.DirectorySeparatorChar));

                result.AddRange(
                    Handle(
                        targetPath,
                        moduleMetadata.FileNames,
                        moduleMetadata.ModuleName,
                        moduleMetadata.VirtualPath,
                        isNeedVersionAppend
                    )
                );
            }

            return result.ToArray();
        }

        private ModuleFile[] Handle(string targetPath, IReadOnlyCollection<string> fileNames, string moduleName, string virtualPath, bool isNeedVersionAppend)
        {
            var result = new List<ModuleFile>();

            if (Directory.Exists(targetPath))
            {
                foreach (var fileName in fileNames)
                {
                    var moduleFile = Path.Join(targetPath, fileName);

                    if (File.Exists(moduleFile))
                    {
                        result.Add(new ModuleFile
                        {
                            WebPath = GetWebPath(moduleFile, moduleName, virtualPath),
                            Version = isNeedVersionAppend ? _fileVersionProvider.GetFileVersion(moduleFile) : null
                        });
                    }
                }
            }

            return result.ToArray();
        }

        private string GetWebPath(string filePath, string moduleName, string virtualPath)
        {
            return $"/Modules/$({moduleName})/{virtualPath}/{Path.GetFileName(filePath)}";
        }
    }
}
