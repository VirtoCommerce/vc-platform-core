using System;
using System.Linq;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportedTypeDefinitionBuilder
    {
        public ExportedTypeDefinition ExportedTypeDefinition { get; private set; }

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

            if (!AbstractTypeFactory<ExportDataQuery>.AllTypeInfos.Any(x => x.Type == typeof(TDataQuery)))
            {
                AbstractTypeFactory<ExportDataQuery>.RegisterType<TDataQuery>();
            }

            return new ExportedTypeDefinitionBuilder(new ExportedTypeDefinition()
            {
                TypeName = exportedType.FullName,
                Group = exportedType.Namespace,
                ExportDataQueryType = dataQueryType.Name,
            });
        }

        public static implicit operator ExportedTypeDefinition(ExportedTypeDefinitionBuilder builder)
        {
            return builder.ExportedTypeDefinition;
        }
    }
}
