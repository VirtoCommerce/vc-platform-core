using System;
using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Data.Services
{
    public class ExportedTypeDefinitionBuilder
    {
        public ExportedTypeDefinition ExportedTypeDefinition { get; }

        public ExportedTypeDefinitionBuilder(string exportedTypeName, string group, string exportQueryType)
        {
            ExportedTypeDefinition = new ExportedTypeDefinition()
            {
                TypeName = exportedTypeName,
                Group = group,
                ExportDataQueryType = exportQueryType,
            };
        }

        public ExportedTypeDefinitionBuilder(ExportedTypeDefinition exportedTypeDefinition)
        {
            ExportedTypeDefinition = exportedTypeDefinition ?? throw new ArgumentNullException(nameof(exportedTypeDefinition));
        }

        public static ExportedTypeDefinitionBuilder Build<TExport, TDataQuery>()
        {
            var exportedType = typeof(TExport);
            var dataQueryType = typeof(TDataQuery);

            return new ExportedTypeDefinitionBuilder(exportedType.FullName, exportedType.Namespace, dataQueryType.Name);
        }
    }
}
