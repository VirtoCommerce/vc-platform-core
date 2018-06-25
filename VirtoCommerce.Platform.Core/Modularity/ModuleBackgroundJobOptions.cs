using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.Platform.Core.Modularity
{
    public class ModuleBackgroundJobOptions
    {
        public ModuleAction Action { get; set; }
        public ModuleDescriptor[] Modules { get; set; }
    }
}
