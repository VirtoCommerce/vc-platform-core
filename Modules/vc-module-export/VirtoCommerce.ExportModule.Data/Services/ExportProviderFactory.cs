using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportProviderFactory : IExportProviderFactory
    {
        private readonly IEnumerable<Func<IExportProviderConfiguration, ExportedTypePropertyInfo[], IExportProvider>> _providerFactories;

        public ExportProviderFactory(IEnumerable<Func<IExportProviderConfiguration, ExportedTypePropertyInfo[], IExportProvider>> providerFactories)
        {
            _providerFactories = providerFactories;
        }

        public virtual IExportProvider CreateProvider(string name, IExportProviderConfiguration config, ExportedTypePropertyInfo[] includedColumns)
        {
            var result = _providerFactories.FirstOrDefault(x => x(new EmptyProviderConfiguration(), includedColumns).TypeName.EqualsInvariant(name));

            return result != null ? result(config, includedColumns) : null;
        }
    }
}
