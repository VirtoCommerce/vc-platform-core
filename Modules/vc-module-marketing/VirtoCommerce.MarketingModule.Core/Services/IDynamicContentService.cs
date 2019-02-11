using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IDynamicContentService
    {
        Task<DynamicContentFolder[]> GetFoldersByIdsAsync(string[] ids);
        Task SaveFoldersAsync(DynamicContentFolder[] folders);
        Task DeleteFoldersAsync(string[] ids);

        Task<DynamicContentItem[]> GetContentItemsByIdsAsync(string[] ids);
        Task SaveContentItemsAsync(DynamicContentItem[] items);
        Task DeleteContentItemsAsync(string[] ids);

        Task<DynamicContentPlace[]> GetPlacesByIdsAsync(string[] ids);
        Task SavePlacesAsync(DynamicContentPlace[] places);
        Task DeletePlacesAsync(string[] ids);

        Task<DynamicContentPublication[]> GetPublicationsByIdsAsync(string[] ids);
        Task SavePublicationsAsync(DynamicContentPublication[] publications);
        Task DeletePublicationsAsync(string[] ids);
    }
}
