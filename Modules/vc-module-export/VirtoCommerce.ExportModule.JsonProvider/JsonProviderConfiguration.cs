using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.JsonProvider
{
    public class JsonProviderConfiguration : IExportProviderConfiguration
    {
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
#if DEBUG
            Formatting = Formatting.Indented,
#endif
        };
    }
}
