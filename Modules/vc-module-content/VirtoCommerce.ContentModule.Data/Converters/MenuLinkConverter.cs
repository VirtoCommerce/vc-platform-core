using System;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Converters
{
    public static class MenuLinkConverter
    {
        /// <summary>
        /// Patch changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Patch(this MenuLink source, MenuLink target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException("target");

            target.AssociatedObjectId = source.AssociatedObjectId;
            target.AssociatedObjectName = source.AssociatedObjectName;
            target.AssociatedObjectType = source.AssociatedObjectType;

            target.Priority = source.Priority;
            target.Title = source.Title;
            target.Url = source.Url;

        }

        public static MenuLinkEntity FromModel(this MenuLink link)
        {
            var linkEntity = AbstractTypeFactory<MenuLinkEntity>.TryCreateInstance();

            linkEntity.Id = link.Id;
            linkEntity.Title = link.Title;
            linkEntity.Url = link.Url;
            linkEntity.Priority = link.Priority;
            linkEntity.MenuLinkListId = link.MenuLinkListId;
            linkEntity.AssociatedObjectId = link.AssociatedObjectId;
            linkEntity.AssociatedObjectName = link.AssociatedObjectName;
            linkEntity.AssociatedObjectType = link.AssociatedObjectType;

            return linkEntity;
        }


        public static MenuLink ToModel(this MenuLinkEntity linkEntity)
        {
            var link = AbstractTypeFactory<MenuLink>.TryCreateInstance();

            link.Id = linkEntity.Id;
            link.Title = linkEntity.Title;
            link.Url = linkEntity.Url;
            link.Priority = linkEntity.Priority;
            link.MenuLinkListId = linkEntity.MenuLinkListId;
            link.AssociatedObjectId = linkEntity.AssociatedObjectId;
            link.AssociatedObjectName = linkEntity.AssociatedObjectName;
            link.AssociatedObjectType = linkEntity.AssociatedObjectType;

            return link;
        }

    }
}
