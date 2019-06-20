using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportProviderFactory : IExportProviderFactory
    {
        private readonly IEnumerable<Func<IExportProviderConfiguration, Stream, IExportProvider>> _providerFactories;

        public ExportProviderFactory(IEnumerable<Func<IExportProviderConfiguration, Stream, IExportProvider>> providerFactories)
        {
            _providerFactories = providerFactories;
        }

        public virtual IExportProvider CreateProvider(string name, IExportProviderConfiguration config, Stream outputStream)
        {
            // Not good to instantiate MemoryStream and EmptyProviderConfiguration, but that creation with fake objects needs to be done for non relying on IExportProvider implementation
            var result = _providerFactories.FirstOrDefault(x =>
            {
                using (var ms = new MemoryStream())
                {
                    return x(new EmptyProviderConfiguration(), ms).TypeName.EqualsInvariant(name);
                }
            });

            return result != null ? result(config, outputStream) : null;
        }
    }
}
