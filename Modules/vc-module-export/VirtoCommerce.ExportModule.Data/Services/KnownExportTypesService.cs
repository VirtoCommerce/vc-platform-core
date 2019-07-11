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

        public ExportedTypeDefinition RegisterType(string exportedTypeName, string group, string exportQueryType)
        {
            if (!_knownExportTypes.TryGetValue(exportedTypeName, out var result))
            {
                _knownExportTypes.TryAdd(exportedTypeName,
                    new ExportedTypeDefinition()
                    {
                        TypeName = exportedTypeName,
                        Group = group,
                        ExportDataQueryType = exportQueryType,
                    }
                );
            }
            return _knownExportTypes[exportedTypeName];
        }

        public ExportedTypeDefinition ResolveExportedTypeDefinition(string typeName)
        {
            return _knownExportTypes.Values.FirstOrDefault(x => x.TypeName.EqualsInvariant(typeName));
        }
    }
}
