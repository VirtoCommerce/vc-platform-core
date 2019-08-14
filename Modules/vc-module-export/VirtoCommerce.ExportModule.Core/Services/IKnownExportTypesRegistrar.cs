using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface for implementing a metadata registrar for exportable entities.
    /// </summary>
    public interface IKnownExportTypesRegistrar
    {
        ExportedTypeDefinition[] GetRegisteredTypes();
        /// <summary>
        /// Registers type metadata of exportable entity.
        /// </summary>
        /// <param name="exportedTypeDefinition"></param>
        /// <returns></returns>
        ExportedTypeDefinition RegisterType(ExportedTypeDefinition exportedTypeDefinition);
    }
}
