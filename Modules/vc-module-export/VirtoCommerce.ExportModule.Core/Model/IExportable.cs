using System;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Basic interface to implement exportаble entities
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
