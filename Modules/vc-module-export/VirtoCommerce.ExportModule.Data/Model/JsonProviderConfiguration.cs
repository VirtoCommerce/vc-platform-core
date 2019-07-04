using Newtonsoft.Json;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Model
{
    public class JsonProviderConfiguration : IExportProviderConfiguration
    {
        public JsonSerializerSettings Settings { get; set; }
    }
}
