using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core2.Model
{
    public class EditorialReview : AuditableEntity, IHasLanguage, ICloneable, IInheritable
    {
        public string Content { get; set; }
        public string ReviewType { get; set; }

        #region ILanguageSupport Members
        public string LanguageCode { get; set; }
        #endregion

        #region IInheritable Members
        public bool IsInherited { get; private set; }
        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is EditorialReview parentReview)
            {
                Id = null;
                IsInherited = true;
                LanguageCode = parentReview.LanguageCode;
                Content = parentReview.Content;
                ReviewType = parentReview.ReviewType;
            }
        }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
