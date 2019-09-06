using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class MenuLink : AuditableEntity, IHasOuterId, ICloneable
    {

        /// <summary>
        /// Title of menu link element, displayed as link text or link title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url of menu link element, inserts in href attribute of link
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// Priority of menu link element, the higher the value, the higher in the list
        /// </summary>
        public int Priority { get; set; }

        public string MenuLinkListId { get; set; }

        /// <summary>
        /// Each link element can have an associated object like Product, Category, Promotion, etc.
        /// Is a primary key of associated object
        /// </summary>
        public string AssociatedObjectId { get; set; }
        /// <summary>
        /// Display name for associated object
        /// </summary>
        public string AssociatedObjectName { get; set; }
        /// <summary>
        /// Associated object type
        /// </summary>
        public string AssociatedObjectType { get; set; }

        public string[] SecurityScopes { get; set; }

        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as MenuLink;

            return result;
        }

        #endregion
    }
}
