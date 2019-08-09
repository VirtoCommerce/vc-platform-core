using System.Collections.Concurrent;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class KnownExportTypesService : IKnownExportTypesRegistrar, IKnownExportTypesResolver
    {
        private readonly ConcurrentDictionary<string, ExportedTypeDefinition> _knownExportTypes = new ConcurrentDictionary<string, ExportedTypeDefinition>();

        public ExportedTypeDefinition[] GetRegisteredTypes()
        {
            return _knownExportTypes.Values.ToArray();
        }

        public ExportedTypeDefinition RegisterType(ExportedTypeDefinition exportedTypeDefinition)
        {
            var exportedTypeName = exportedTypeDefinition.TypeName;

            if (!_knownExportTypes.ContainsKey(exportedTypeName))
            {
                _knownExportTypes.TryAdd(exportedTypeName, exportedTypeDefinition);
            }

            return _knownExportTypes[exportedTypeName];
        }

        public ExportedTypeDefinition ResolveExportedTypeDefinition(string typeName)
        {
            return _knownExportTypes.Values.FirstOrDefault(x => x.TypeName.EqualsInvariant(typeName));
        }
    }
}
