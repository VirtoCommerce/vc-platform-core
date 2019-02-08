using System.Threading.Tasks;
using VirtoCommerce.MarketingModule.Core.Model.DynamicContent;

namespace VirtoCommerce.MarketingModule.Core.Services
{
    public interface IDynamicContentService
    {
        DynamicContentFolder[] GetFoldersByIds(string[] ids);
        void SaveFolders(DynamicContentFolder[] folders);
        void DeleteFolders(string[] ids);

        Task<DynamicContentItem[]> GetContentItemsByIds(string[] ids);
        Task SaveContentItems(DynamicContentItem[] items);
        Task DeleteContentItems(string[] ids);

        DynamicContentPlace[] GetPlacesByIds(string[] ids);
        void SavePlaces(DynamicContentPlace[] places);
        void DeletePlaces(string[] ids);

        DynamicContentPublication[] GetPublicationsByIds(string[] ids);
        void SavePublications(DynamicContentPublication[] publications);
        void DeletePublications(string[] ids);
    }
}
