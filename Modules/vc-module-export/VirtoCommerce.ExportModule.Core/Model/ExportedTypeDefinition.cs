using System;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public class ExportedTypeDefinition
    {
        public string TypeName { get; set; }
        public ExportedTypeMetadata MetaData { get; set; }
        public Func<ExportDataQuery, IPagedDataSource> ExportedDataSourceFactory;

        public ExportedTypeDefinition WithDataSourceFactory(Func<ExportDataQuery, IPagedDataSource> factory)
        {
            ExportedDataSourceFactory = factory;
            return this;
        }

        public ExportedTypeDefinition WithMetadata(ExportedTypeMetadata metadata)
        {
            MetaData = metadata;
            return this;
        }
    }
}
