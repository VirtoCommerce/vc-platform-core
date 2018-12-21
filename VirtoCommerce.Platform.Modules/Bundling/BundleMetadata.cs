using System.Collections.Generic;

namespace VirtoCommerce.Platform.Modules.Bundling
{
    public class BundleMetadata
    {

        public string BundleName { get; set; }

        public string VendorName { get; set; }

        public IReadOnlyCollection<ModuleMetadata> ModulesMetadata { get; set; }
    }
}
