using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class KnownExportTypesService : IKnownExportTypesRegistrar, IKnownExportTypesResolver
    {
        private readonly Dictionary<Type, ExportedTypeDefinition> _knownExportTypes = new Dictionary<Type, ExportedTypeDefinition>();

        public ExportedTypeDefinition RegisterType<T>()
        {
            var type = typeof(T);

            if (_knownExportTypes.TryGetValue(type, out var result))
            {
                _knownExportTypes.Add(type, new ExportedTypeDefinition()
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
