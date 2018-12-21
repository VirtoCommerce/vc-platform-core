namespace VirtoCommerce.Platform.Modules.Bundling
{
    public interface ICollector
    {
        ModuleFile[] Collect(bool isNeedVersionAppend);
    }
}
