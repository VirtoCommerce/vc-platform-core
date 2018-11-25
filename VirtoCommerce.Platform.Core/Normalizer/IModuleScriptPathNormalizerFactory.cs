namespace VirtoCommerce.Platform.Core.Normalizer
{
    public interface IModuleScriptPathNormalizerFactory
    {
        IModuleScriptPathNormalizer Create(string path, string moduleName, string moduleFolder);
    }
}
