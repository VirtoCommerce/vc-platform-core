namespace VirtoCommerce.Platform.Core.ModuleScriptCollector
{
    public interface IScriptCollector
    {
        ModuleScript[] Collect(bool isNeedVersionAppend);
    }
}
