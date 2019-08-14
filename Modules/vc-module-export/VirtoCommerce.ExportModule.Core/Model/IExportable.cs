using System;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Interface to implement exportаble entities.
    /// Common (default) set of properties for universal viewing.
    /// </summary>
    public interface IExportable : ICloneable
    {
        string Name { get; set; }
        string Code { get; set; }
        string ImageUrl { get; set; }
        string Parent { get; set; }
        string Type { get; set; }
    }
}
