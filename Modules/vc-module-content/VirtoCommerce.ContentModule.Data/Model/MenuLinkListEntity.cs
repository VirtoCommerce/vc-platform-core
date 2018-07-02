using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Converters;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class MenuLinkListEntity : AuditableEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string StoreId { get; set; }

        public string Language { get; set; }
        public virtual ObservableCollection<MenuLinkEntity> MenuLinks { get; set; } = new ObservableCollection<MenuLinkEntity>();

        public void Patch(MenuLinkListEntity target)
        {
            Language = target.Language;
            Name = target.Name;

            if( !MenuLinks.IsNullCollection())
            {
                MenuLinks.Patch(target.MenuLinks, (sourceLink, targetLink) => sourceLink.Patch(targetLink));
            }
        }


        public MenuLinkList ToModel(MenuLinkList menuLinkList)
        {
            menuLinkList.Id = Id;
            menuLinkList.Name = Name;
            menuLinkList.StoreId = StoreId;
            menuLinkList.Language = Language;

            if (MenuLinks.Any())
            {
                menuLinkList.MenuLinks = MenuLinks.OrderByDescending(l => l.Priority).Select(s => s.ToModel()).ToArray();
            }

            return menuLinkList;
        }
    }
}
