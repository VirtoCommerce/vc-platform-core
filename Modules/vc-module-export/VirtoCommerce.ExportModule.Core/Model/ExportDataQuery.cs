using System;
using Newtonsoft.Json;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public abstract class ExportDataQuery : ValueObject
    {
        [JsonProperty("exportTypeName")]
        public string ExportTypeName => GetType().Name;
        public string Keyword { get; set; }
        public string[] ObjectIds { get; set; } = new string[] { };
        public string Sort { get; set; }
        public ExportedTypeColumnInfo[] IncludedColumns { get; set; } = Array.Empty<ExportedTypeColumnInfo>();
        public int? Skip { get; set; }
        public int? Take { get; set; } = 50;
    }
}
