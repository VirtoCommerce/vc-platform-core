using Microsoft.Extensions.Options;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Core.Normalizer
{
    public class ModuleScriptPathNormalizerFactory : IModuleScriptPathNormalizerFactory
    {
        private readonly LocalStorageModuleCatalogOptions _localStorageModuleCatalogOptions;

        public ModuleScriptPathNormalizerFactory(
            IOptions<LocalStorageModuleCatalogOptions> localStorageModuleCatalogOptions)
        {
            _localStorageModuleCatalogOptions = localStorageModuleCatalogOptions.Value;
        }
        public IModuleScriptPathNormalizer Create(string path, string moduleName, string moduleFolder)
        {
            return new ModuleScriptPathNormalizer(_localStorageModuleCatalogOptions, path, moduleName, moduleFolder);
        }
    }
}
