using System.Collections.Generic;
using VirtoCommerce.ExportModule.Core.Model;
using VirtoCommerce.ExportModule.Core.Services;

namespace VirtoCommerce.CatalogModule.Data.ExportImport
{
    public interface ICatalogExportPagedDataSourceFactory : IPagedDataSourceFactory
    {
        IEnumerable<IPagedDataSource> GetAllFullExportPagedDataSources(CatalogFullExportDataQuery query);
    }
}
