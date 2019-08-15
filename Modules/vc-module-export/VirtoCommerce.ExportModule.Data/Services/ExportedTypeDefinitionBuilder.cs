using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportedTypeDefinitionBuilder
    {
        public ExportedTypeDefinition ExportedTypeDefinition { get; }


        /// <summary>
        /// Creates <see cref="ExportedTypeDefinitionBuilder"/> with ExportedTypeDefinition instance.
        /// </summary>
        /// <param name="exportedTypeDefinition">Definition to build.</param>
        public ExportedTypeDefinitionBuilder(ExportedTypeDefinition exportedTypeDefinition)
        {
            ExportedTypeDefinition = exportedTypeDefinition ?? throw new ArgumentNullException(nameof(exportedTypeDefinition));
        }

        /// <summary>
        /// Creates <see cref="ExportedTypeDefinitionBuilder"/> with definition with <typeparamref name="TExportable"/> type <see cref="Type.FullName"/> as <see cref="ExportedTypeDefinition.TypeName"/>,
        /// <see cref="Type.Namespace"/> as <see cref="ExportedTypeDefinition.Group"/> and ExportDataQuery type name as <see cref="ExportedTypeDefinition.ExportDataQueryType"/>.
        /// </summary>
        /// <typeparam name="TExportable">Exportable entity type.</typeparam>
        /// <typeparam name="TDataQuery">Type to query entities.</typeparam>
        /// <returns></returns>
        public static ExportedTypeDefinitionBuilder Build<TExportable, TDataQuery>() where TDataQuery : ExportDataQuery
        {
            var exportedType = typeof(TExportable);
            var dataQueryType = typeof(TDataQuery);

            return new ExportedTypeDefinitionBuilder(new ExportedTypeDefinition()
            {
                TypeName = exportedType.FullName,
                Group = exportedType.Namespace,
                ExportDataQueryType = dataQueryType.Name,
            });
        }
    }
}
