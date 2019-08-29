using VirtoCommerce.ExportModule.Core.Model;

namespace VirtoCommerce.ExportModule.Core.Services
{
    public interface IPagedDataSourceFactory
    {
        IPagedDataSource Create(ExportDataQuery dataQuery);
    }
}
