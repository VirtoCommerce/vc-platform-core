using System;
using System.IO;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Core.ModuleScriptCollector.Normalizer
{
    public class ModuleScriptPathNormalizer : IModuleScriptPathNormalizer
    {
        private readonly string _rootPath;
        private readonly string _path;
        private readonly string _moduleName;
        private readonly string _moduleFolder;

        public ModuleScriptPathNormalizer(LocalStorageModuleCatalogOptions localStorageModuleCatalogOptions, string path, string moduleName, string moduleFolder)
        {
            _rootPath = localStorageModuleCatalogOptions.DiscoveryPath;
            _path = path;
            _moduleName = moduleName;
            _moduleFolder = moduleFolder;
        }
        public string Normalize(string fileFullPath)
        {
            var p = GetRelativePath(fileFullPath, GetRootFolder(_path));
            return p;
        }

        //TODO: Everything below should be rewritten
        string GetRelativePath(string fullPath, string moduleFolder)
        {
            return fullPath.Replace(_rootPath, "")
                .Replace(moduleFolder, "Modules")
                .Replace("\\", "/")
                .Replace($"{_moduleFolder}", $"$({_moduleName})");
        }

        string GetRootFolder(string path)
        {
            while (true)
            {
                string temp = Path.GetDirectoryName(path);
                if (String.IsNullOrEmpty(temp))
                    break;
                path = temp;
            }
            return path;
        }
    }
}
