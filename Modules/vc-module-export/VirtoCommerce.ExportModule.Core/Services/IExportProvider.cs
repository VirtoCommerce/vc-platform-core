using System;
using System.IO;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface for export provider implementation.
    /// The export provider allows to write object using the given TextWriter.
    /// </summary>
    public interface IExportProvider : IDisposable
    {
        /// <summary>
        /// Provider name.
        /// </summary>
        string TypeName { get; }
        /// <summary>
        /// Extension for resulting export file.
        /// </summary>
        string ExportedFileExtension { get; }
        /// <summary>
        /// Returns <see cref="true"/> if provider supports only plain tabular objects (without nested entities).
        /// </summary>
        bool IsTabular { get; }
        /// <summary>
        /// Returns provider configuration.
        /// </summary>
        IExportProviderConfiguration Configuration { get; }

        /// <summary>
        /// Writes object using the given TextWriter.
        /// </summary>
        /// <param name="writer">TextWriter to write to.</param>
        /// <param name="objectToRecord">Object to write.</param>
        void WriteRecord(TextWriter writer, IExportable objectToRecord);
    }
}
