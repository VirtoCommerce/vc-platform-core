using System.Collections.Generic;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class ModuleMetadata
    {
        public IReadOnlyCollection<string> FileNames { get; set; }

        public string VirtualPath { get; set; }

        public string ModuleName { get; set; }

        public string FullPhysicalModulePath { get; set; }
    }
}
