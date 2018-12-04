using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Core.ModuleFileCollector.Normalizer
{
    public class ModuleFilePathNormalizerFactory : IModuleFilePathNormalizerFactory
    {
        private readonly LocalStorageModuleCatalogOptions _localStorageModuleCatalogOptions;

        public ModuleFilePathNormalizerFactory(
            IOptions<LocalStorageModuleCatalogOptions> localStorageModuleCatalogOptions)
        {
            _localStorageModuleCatalogOptions = localStorageModuleCatalogOptions.Value;
        }
        public IModuleFilePathNormalizer Create(string path, string moduleName, string moduleFolder)
        {
            return new ModuleFilePathNormalizer(_localStorageModuleCatalogOptions, path, moduleName, moduleFolder);
        }
    }
}
