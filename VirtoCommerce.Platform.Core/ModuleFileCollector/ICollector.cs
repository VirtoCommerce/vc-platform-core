namespace VirtoCommerce.Platform.Core.ModuleFileCollector
{
    public interface ICollector
    {
        ModuleFile[] Collect(bool isNeedVersionAppend);
    }
}
