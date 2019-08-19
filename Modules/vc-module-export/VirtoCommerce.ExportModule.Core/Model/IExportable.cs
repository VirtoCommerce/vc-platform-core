using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    /// <summary>
    /// Interface to implement exportаble entities.
    /// </summary>
    public interface IExportable : ICloneable, IEntity
    {
    }
}
