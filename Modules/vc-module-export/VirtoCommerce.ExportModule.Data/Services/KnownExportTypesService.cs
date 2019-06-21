using System;
using System.Collections.Concurrent;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class KnownExportTypesService : IKnownExportTypesRegistrar, IKnownExportTypesResolver
    {
        private readonly ConcurrentDictionary<Type, ExportedTypeDefinition> _knownExportTypes = new ConcurrentDictionary<Type, ExportedTypeDefinition>();

        public ExportedTypeDefinition[] GetRegisteredTypes()
        {
            return _knownExportTypes.Values.ToArray();
        }

        public ExportedTypeDefinition RegisterType<T>()
        {
            var type = typeof(T);

            if (!_knownExportTypes.TryGetValue(type, out var result))
            {
                _knownExportTypes.TryAdd(type, new ExportedTypeDefinition()
                {
                    TypeName = type.FullName,
                    MetaData = ExportedTypeMetadata.GetFromType<T>(),
                });
            }
            return _knownExportTypes[type];
        }

        public ExportedTypeDefinition ResolveExportedTypeDefinition(string typeName)
        {
            return _knownExportTypes.Values.FirstOrDefault(x => x.TypeName.EqualsInvariant(typeName));
        }
    }
}
