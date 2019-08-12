using System;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IExportViewable : ICloneable
    {
        string Name { get; set; }
        string Code { get; set; }
        string ImageUrl { get; set; }
        string Parent { get; set; }
        string Type { get; set; }
    }
}
