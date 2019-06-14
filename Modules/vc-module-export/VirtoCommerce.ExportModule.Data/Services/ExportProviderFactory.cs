using System;
using System.IO;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportProviderFactory : IExportProviderFactory
    {
        public virtual IExportProvider CreateProvider(string name, IExportProviderConfiguration config, Stream outputStream)
        {
            IExportProvider result = null;

            // TODO: Probably could make some providers registrar to have an ability to add providers. Then search them by name and instantiate using Reflection

            switch (name)
            {
                case nameof(JsonExportProvider):
                    result = new JsonExportProvider(outputStream, config);
                    break;
                case nameof(CsvExportProvider):
                    result = new CsvExportProvider(outputStream, config);
                    break;
                default:
                    throw new NotSupportedException($"Provider \"{name}\" is not supported.");
            }

            return result;
        }
    }
}
