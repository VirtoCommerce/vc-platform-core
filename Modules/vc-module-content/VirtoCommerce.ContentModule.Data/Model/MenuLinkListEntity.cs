using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class MenuLinkListEntity : AuditableEntity, IHasOuterId
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string StoreId { get; set; }

        public string Language { get; set; }
        [StringLength(128)]
        public string OuterId { get; set; }

        #region Navigation Properties

        public virtual ObservableCollection<MenuLinkEntity> MenuLinks { get; set; } = new ObservableCollection<MenuLinkEntity>();

        #endregion

        public void Patch(MenuLinkListEntity target)
        {
            target.Language = Language;
            target.Name = Name;
            target.StoreId = StoreId;

            if (!MenuLinks.IsNullCollection())
            {
                MenuLinks.Patch(target.MenuLinks, (sourceLink, targetLink) => sourceLink.Patch(targetLink));
            }
        }

        public MenuLinkList ToModel(MenuLinkList menuLinkList)
        {
            menuLinkList.Id = Id;
            menuLinkList.OuterId = OuterId;

            menuLinkList.Name = Name;
            menuLinkList.StoreId = StoreId;
            menuLinkList.Language = Language;

            if (MenuLinks.Any())
            {
                menuLinkList.MenuLinks = MenuLinks.OrderByDescending(l => l.Priority)
                    .Select(s => s.ToModel(AbstractTypeFactory<MenuLink>.TryCreateInstance()))
                    .ToArray();
            }

            return menuLinkList;
        }

        public MenuLinkListEntity FromModel(MenuLinkList menuLinkList, PrimaryKeyResolvingMap pkMap)
        {
            if (menuLinkList == null)
                throw new ArgumentNullException(nameof(menuLinkList));

            pkMap.AddPair(menuLinkList, this);

            Id = menuLinkList.Id;
            OuterId = menuLinkList.OuterId;

            StoreId = menuLinkList.StoreId;
            Name = menuLinkList.Name;
            Language = menuLinkList.Language;
            ModifiedDate = menuLinkList.ModifiedDate;
            ModifiedBy = menuLinkList.ModifiedBy;

            foreach (var link in menuLinkList.MenuLinks)
            {
                MenuLinks.Add(AbstractTypeFactory<MenuLinkEntity>.TryCreateInstance().FromModel(link));
            }

            return this;
        }
    }
}
