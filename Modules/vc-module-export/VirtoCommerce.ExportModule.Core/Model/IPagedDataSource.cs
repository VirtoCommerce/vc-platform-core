using System.Collections;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IPagedDataSource
    {
        int PageSize { get; set; }
        int CurrentPageNumber { get; set; }
        ExportDataQuery DataQuery { get; set; }
        int GetTotalCount();        
        IEnumerable FetchNextPage();        
    }
}
