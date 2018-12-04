namespace VirtoCommerce.Platform.Core.ModuleFileCollector.Normalizer
{
    public interface IModuleFilePathNormalizerFactory
    {
        IModuleFilePathNormalizer Create(string path, string moduleName, string moduleFolder);
    }
}
