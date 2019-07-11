using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IKnownExportTypesRegistrar
    {
        ExportedTypeDefinition[] GetRegisteredTypes();
        ExportedTypeDefinition RegisterType(string exportedTypeName, string group, string exportQueryType);
    }
}
