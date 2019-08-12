using System.Collections.Generic;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IPagedDataSource
    {
        ExportDataQuery DataQuery { get; set; }
        int GetTotalCount();
        IEnumerable<IExportable> FetchNextPage();
    }
}
