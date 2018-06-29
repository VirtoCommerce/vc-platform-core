using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Converters
{
    public static class MenuLinkListConverter
    {
        /// <summary>
        /// Patch changes
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Patch(this MenuLinkList source, MenuLinkList target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException("target");

            target.Language = source.Language;
            target.Name = source.Name;

            if (!source.MenuLinks.IsNullCollection())
            {
                source.MenuLinks.Patch(target.MenuLinks, (sourceLink, targetLink) => sourceLink.Patch(targetLink));
            }
        }
    }
}
