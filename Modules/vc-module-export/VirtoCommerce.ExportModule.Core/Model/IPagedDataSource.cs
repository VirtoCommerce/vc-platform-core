using System.Collections;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IPagedDataSource
    {
        int PageSize { get; set; }
        int CurrentPageNumber { get; }
        int GetTotalCount();
        IEnumerable FetchNextPage();
    }
}
