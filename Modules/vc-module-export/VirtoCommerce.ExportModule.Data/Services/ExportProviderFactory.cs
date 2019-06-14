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
            throw new NotImplementedException();
        }
    }
}
