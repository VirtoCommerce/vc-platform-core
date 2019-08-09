using System;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;
using VirtoCommerce.ExportModule.Data.Services;

namespace VirtoCommerce.ExportModule.Data.Extensions
{
    public static class ExportedTypeDefinitionBuilderExtensions
    {
        public static ExportedTypeDefinitionBuilder WithDataSourceFactory(this ExportedTypeDefinitionBuilder builder, Func<ExportDataQuery, IPagedDataSource> factory)
        {
            builder.ExportedTypeDefinition.ExportedDataSourceFactory = factory;
            return builder;
        }

        public static ExportedTypeDefinitionBuilder WithMetadata(this ExportedTypeDefinitionBuilder builder, ExportedTypeMetadata metadata)
        {
            builder.ExportedTypeDefinition.MetaData = metadata;
            return builder;
        }

        public static ExportedTypeDefinitionBuilder WithTabularMetadata(this ExportedTypeDefinitionBuilder builder, ExportedTypeMetadata tabularMetadata)
        {
            builder.ExportedTypeDefinition.TabularMetaData = tabularMetadata;
            return builder;
        }

        public static ExportedTypeDefinitionBuilder WithTabularDataConverter(this ExportedTypeDefinitionBuilder builder, ITabularDataConverter tabularDataConverter)
        {
            builder.ExportedTypeDefinition.TabularDataConverter = tabularDataConverter;
            return builder;
        }
    }
}
