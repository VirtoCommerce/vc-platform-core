using System;
using System.Collections.Generic;

namespace VirtoCommerce.Platform.Modules.Abstractions
{
    public interface IModuleInstaller
    {
        void Install(IEnumerable<ManifestModuleInfo> modules, IProgress<ProgressMessage> progress);
        void Uninstall(IEnumerable<ManifestModuleInfo> modules, IProgress<ProgressMessage> progress);
    }
}
