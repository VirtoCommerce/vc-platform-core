using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    /// <summary>
    /// Interface for implementing a metadata resolver for exportable entities.
    /// </summary>
    public interface IKnownExportTypesResolver
    {
        /// <summary>
        /// Returns metadata about specified type of exportable entity.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        ExportedTypeDefinition ResolveExportedTypeDefinition(string typeName);
    }
}
