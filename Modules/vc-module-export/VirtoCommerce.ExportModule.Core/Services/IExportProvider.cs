using System;
using System.IO;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface for export provider implementation.
    /// The export provider directly writes exportable entity or its tabular representation
    /// </summary>
    public interface IExportProvider : IDisposable
    {
        string TypeName { get; }
        string ExportedFileExtension { get; }
        bool IsTabular { get; }
        IExportProviderConfiguration Configuration { get; }

        /// <summary>
        /// Writes exportable entity or its tabular representation
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="objectToRecord"></param>
        void WriteRecord(TextWriter writer, object objectToRecord);
    }
}
