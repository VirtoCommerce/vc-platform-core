using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Platform.Core.VersionProvider;

namespace VirtoCommerce.Platform.Core.ModuleFileCollector
{
    public abstract class FileCollector : ICollector
    {
        private readonly IFileVersionProvider _fileVersionProvider;

        private ICollection<InternalModuleFile> _moduleFiles;

        protected FileCollector(IFileVersionProvider fileVersionProvider)
        {
            _fileVersionProvider = fileVersionProvider;
        }

        protected abstract string ModuleName { get; set; }
        protected abstract string VirtualPath { get; set; }
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

                if (File.Exists(moduleFile))
                {
                    result.Add(new InternalModuleFile
                    {
                        Path = moduleFile,
                        WebPath = GetWebPath(moduleFile)
                    });
                }

                if (File.Exists(moduleVendor))
                {
                    result.Add(new InternalModuleFile
                    {
                        Path = moduleVendor,
                        WebPath = GetWebPath(moduleVendor),
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

        private string GetWebPath(string filePath)
        {
            return $"/Modules/$({ModuleName})/{VirtualPath}/{Path.GetFileName(filePath)}";
        }
    }
}
