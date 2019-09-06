using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class MenuLinkList : AuditableEntity, ICloneable
    {
        /// <summary>
        /// Name of menu link list, can be used as title of list in frontend
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Store identifier, for which the list belongs
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// Locale of this menu link list
        /// </summary>
        public string Language { get; set; }

        public ICollection<MenuLink> MenuLinks { get; set; }

        public string[] SecurityScopes { get; set; }
        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as MenuLinkList;

            result.MenuLinks = MenuLinks?.Select(x => x.Clone()).OfType<MenuLink>().ToList();

            return result;
        }

        #endregion
    }
}
