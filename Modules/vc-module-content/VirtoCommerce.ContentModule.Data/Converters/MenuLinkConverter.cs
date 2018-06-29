using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;

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
    }
}
