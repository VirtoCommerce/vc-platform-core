using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.ModuleFileCollector.Normalizer;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Core.ModuleFileCollector
{
    public abstract class FileCollector : ICollector
    {
        private readonly IModuleFilePathNormalizerFactory _moduleFilePathNormalizerFactory;
        private readonly IFileVersionProvider _fileVersionProvider;

        private ICollection<InternalModuleFile> _moduleFiles;

        protected FileCollector(IModuleFilePathNormalizerFactory moduleFilePathNormalizerFactory, IFileVersionProvider fileVersionProvider)
        {
            _moduleFilePathNormalizerFactory = moduleFilePathNormalizerFactory;
            _fileVersionProvider = fileVersionProvider;
        }

        protected abstract ManifestModuleInfo ModuleInfo { get; set; }
        protected abstract ManifestBundleItem BundleItem { get; set; }
        internal abstract InternalModuleFile[] LoadModulesTargetPaths();

        public ModuleFile[] Collect(bool isNeedVersionAppend)
        {
            _moduleFiles = LoadModulesTargetPaths();

            AddFileVersion(isNeedVersionAppend);

            return _moduleFiles.OfType<ModuleFile>().ToArray();
        }

        internal InternalModuleFile[] Handle(string targetPath, string applicationFilename, string vendorFilename)
        {
            var result = new List<InternalModuleFile>();

            if (Directory.Exists(targetPath))
            {
                var moduleFile = Path.Join(targetPath, applicationFilename);
                var moduleVendor = Path.Join(targetPath, vendorFilename);

                var normalizer = _moduleFilePathNormalizerFactory.Create(BundleItem.VirtualPath, ModuleInfo.ModuleName, ModuleInfo.Assembly.GetName().Name);

                if (File.Exists(moduleFile))
                {
                    result.Add(new InternalModuleFile
                    {
                        Path = moduleFile,
                        WebPath = normalizer.Normalize(moduleFile)
                    });
                }

                if (File.Exists(moduleVendor))
                {
                    result.Add(new InternalModuleFile
                    {
                        Path = moduleVendor,
                        WebPath = normalizer.Normalize(moduleFile),
                        IsVendor = true
                    });
                }
            }

            return result.ToArray();
        }

        internal virtual void AddFileVersion(bool isNeedVersionAppend)
        {
            if (isNeedVersionAppend)
            {
                foreach (var moduleFile in _moduleFiles)
                {
                    moduleFile.Version = _fileVersionProvider.GetFileVersion(moduleFile.Path);
                }
            }
        }
    }
}
