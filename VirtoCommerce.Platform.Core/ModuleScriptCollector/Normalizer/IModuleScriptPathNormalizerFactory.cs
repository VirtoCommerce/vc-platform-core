namespace VirtoCommerce.Platform.Core.ModuleScriptCollector.Normalizer
{
    public interface IModuleScriptPathNormalizerFactory
    {
        IModuleScriptPathNormalizer Create(string path, string moduleName, string moduleFolder);
    }
}
