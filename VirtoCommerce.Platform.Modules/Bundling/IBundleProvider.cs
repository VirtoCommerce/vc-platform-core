using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public interface IBundleProvider
    {
        ModuleFile[] CollectScripts(IReadOnlyCollection<ManifestModuleInfo> modulesInfo, bool isNeedVersionAppend);

        ModuleFile[] CollectStyles(IReadOnlyCollection<ManifestModuleInfo> modulesInfo, bool isNeedVersionAppend);
    }
}
