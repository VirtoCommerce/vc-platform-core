using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.ModuleFileCollector.Normalizer;
using Xunit;

namespace VirtoCommerce.Platform.Tests.UnitTests
{
    public class ModulePathNormalizerTest
    {
        [Fact]
        public void TestNormalizer()
        {
            var rootPath = @"bla\bla\path";
            var path = "bestModuleEver/VirtoCommerce.CatalogModule.Web/Scripts";
            var moduleName = "targetName";
            var moduleFolder = "SomeModuleFolder";
            var fullFilePath = @"bla\bla\path\bestModuleEver\SomeModuleFolder\Scripts\dist\app.js";

            var normalizer = new ModuleFilePathNormalizer(GetLocalStorageModuleCatalogOptions(rootPath), path, moduleName, moduleFolder);

            var result = normalizer.Normalize(fullFilePath);

            Assert.Equal("/Modules/$(targetName)/Scripts/dist/app.js", result);
        }

        private static LocalStorageModuleCatalogOptions GetLocalStorageModuleCatalogOptions(string rootPath)
        {
            return new LocalStorageModuleCatalogOptions
            {
                DiscoveryPath = rootPath
            };
        }
    }
}
