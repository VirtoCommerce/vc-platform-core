using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IKnownExportTypesResolver
    {
        ExportedTypeDefinition ResolveExportedTypeDefinition(string typeName);
    }
}
