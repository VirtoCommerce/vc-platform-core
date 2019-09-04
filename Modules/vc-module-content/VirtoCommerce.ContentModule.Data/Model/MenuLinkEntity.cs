using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class MenuLinkEntity : AuditableEntity, IHasOuterId
    {
        [Required]
        [StringLength(1024)]
        public string Title { get; set; }
        [Required]
        [StringLength(2048)]
        public string Url { get; set; }
        [Required]
        public int Priority { get; set; }
        [StringLength(254)]
        public string AssociatedObjectType { get; set; }
        [StringLength(254)]
        public string AssociatedObjectName { get; set; }
        [StringLength(128)]
        public string AssociatedObjectId { get; set; }

        #region Navigation Properties

        public string MenuLinkListId { get; set; }
        public virtual MenuLinkListEntity MenuLinkList { get; set; }

        #endregion

        [StringLength(128)]
        public string OuterId { get; set; }

        public void Patch(MenuLinkEntity target)
        {
            target.Title = Title;
            target.Url = Url;
            target.Priority = Priority;

            target.AssociatedObjectId = AssociatedObjectId;
            target.AssociatedObjectName = AssociatedObjectName;
            target.AssociatedObjectType = AssociatedObjectType;
        }

        public MenuLink ToModel(MenuLink link)
        {
            link.Id = Id;
            link.OuterId = OuterId;

            link.Title = Title;
            link.Url = Url;
            link.Priority = Priority;
            link.MenuLinkListId = MenuLinkListId;
            link.AssociatedObjectId = AssociatedObjectId;
            link.AssociatedObjectName = AssociatedObjectName;
            link.AssociatedObjectType = AssociatedObjectType;

            return link;
        }

        public MenuLinkEntity FromModel(MenuLink link)
        {
            Id = link.Id;
            OuterId = link.OuterId;

            Title = link.Title;
            Url = link.Url;
            Priority = link.Priority;
            MenuLinkListId = link.MenuLinkListId;
            AssociatedObjectId = link.AssociatedObjectId;
            AssociatedObjectName = link.AssociatedObjectName;
            AssociatedObjectType = link.AssociatedObjectType;

            return this;
        }
    }
}
