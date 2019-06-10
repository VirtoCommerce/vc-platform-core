using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IKnownExportTypesRegistrar
    {
        ExportedTypeDefinition RegisterType<T>();
    }
}
