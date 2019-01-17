namespace VirtoCommerce.Platform.Modules.Bundling
{
    public interface IBundleProvider
    {
        ModuleFile[] Collect(ModuleMetadata[] modulesMetadata, bool isNeedVersionAppend);
    }
}
