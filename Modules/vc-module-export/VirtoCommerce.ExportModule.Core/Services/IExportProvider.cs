using System;
using System.IO;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IExportProvider : IDisposable
    {
        string TypeName { get; }
        string ExportedFileExtension { get; }
        bool IsTabular { get; }
        IExportProviderConfiguration Configuration { get; }

        void WriteRecord(TextWriter writer, object objectToRecord);
    }
}
