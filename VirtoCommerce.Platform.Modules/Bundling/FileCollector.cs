using System.Collections.Generic;
using System.IO;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public abstract class FileCollector : ICollector
    {
        private readonly IFileVersionProvider _fileVersionProvider;

        protected FileCollector(IFileVersionProvider fileVersionProvider)
        {
            _fileVersionProvider = fileVersionProvider;
        }

        protected abstract BundleMetadata GetMetadata();

        public ModuleFile[] Collect(bool isNeedVersionAppend)
        {
            var metadata = GetMetadata();

            return LoadModulesTargetPaths(metadata, isNeedVersionAppend);
        }

        protected virtual ModuleFile[] LoadModulesTargetPaths(BundleMetadata bundleMetadata, bool isNeedVersionAppend)
        {
            var result = new List<ModuleFile>();

            foreach (var moduleMetadata in bundleMetadata.ModulesMetadata)
            {

                if (moduleMetadata.VirtualPath == null)
                {
                    continue;
                }

                var targetPath = Path.Join(moduleMetadata.FullPhysicalModulePath, moduleMetadata.VirtualPath.Replace('/', Path.DirectorySeparatorChar));

                result.AddRange(
                    Handle(
                        targetPath,
                        bundleMetadata.BundleName,
                        bundleMetadata.VendorName,
                        moduleMetadata.ModuleName,
                        moduleMetadata.VirtualPath,
                        isNeedVersionAppend
                    )
                );
            }

            return result.ToArray();
        }

        private ModuleFile[] Handle(string targetPath, string applicationFileName, string vendorFileName, string moduleName, string virtualPath, bool isNeedVersionAppend)
        {
            var result = new List<ModuleFile>();

            if (Directory.Exists(targetPath))
            {
                var moduleFile = Path.Join(targetPath, applicationFileName);
                var moduleVendor = Path.Join(targetPath, vendorFileName);

                if (File.Exists(moduleFile))
                {
                    result.Add(new ModuleFile
                    {
                        WebPath = GetWebPath(moduleFile, moduleName, virtualPath),
                        Version = isNeedVersionAppend ? _fileVersionProvider.GetFileVersion(moduleFile) : null
                    });
                }

                if (File.Exists(moduleVendor))
                {
                    result.Add(new ModuleFile
                    {
                        WebPath = GetWebPath(moduleVendor, moduleName, virtualPath),
                        Version = isNeedVersionAppend ? _fileVersionProvider.GetFileVersion(moduleFile) : null,
                        IsVendor = true
                    });
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
