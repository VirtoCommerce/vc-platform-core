using System.Collections.Generic;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IPagedDataSource
    {
        int CurrentPageNumber { get; }
        int PageSize { get; set; }
        int GetTotalCount();
        IEnumerable<IExportable> FetchNextPage();
    }
}
