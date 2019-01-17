using System.Collections.Generic;
using System.IO;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class BundleProvider : IBundleProvider
    {
        private readonly IFileVersionProvider _fileVersionProvider;

        public BundleProvider(IFileVersionProvider fileVersionProvider)
        {
            _fileVersionProvider = fileVersionProvider;
        }

        public ModuleFile[] Collect(ModuleMetadata[] modulesMetadata, bool isNeedVersionAppend)
        {
            var result = new List<ModuleFile>();

            foreach (var moduleMetadata in modulesMetadata)
            {
                if (string.IsNullOrEmpty(moduleMetadata.VirtualPath))
                {
                    continue;
                }

                var targetPath = Path.Join(moduleMetadata.FullPhysicalModulePath, moduleMetadata.VirtualPath.Replace('/', Path.DirectorySeparatorChar));

                result.AddRange(Handle(targetPath, moduleMetadata.FileNames, moduleMetadata.ModuleName, moduleMetadata.VirtualPath, isNeedVersionAppend));
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
