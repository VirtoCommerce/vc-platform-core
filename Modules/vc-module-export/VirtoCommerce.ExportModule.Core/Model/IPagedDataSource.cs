using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExportModule.Core.Model
{
    public interface IPagedDataSource
    {
        int PageSize { get; set; }
        int CurrentPageNumber { get; set; }
        int GetTotalCount();
        IEnumerable<Entity> FetchNextPage();
    }
}
