using System;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class EditorialReview : AuditableEntity, IHasLanguage, ICloneable, IInheritable
    {
        public string Content { get; set; }
        public string ReviewType { get; set; }

        #region ILanguageSupport Members
        public string LanguageCode { get; set; }
        #endregion

        #region IInheritable Members
        public bool IsInherited { get; set; }
        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is EditorialReview parentBase)
            {
                Id = null;
                IsInherited = true;
                LanguageCode = parentBase.LanguageCode;
                Content = parentBase.Content;
                ReviewType = parentBase.ReviewType;
            }
        }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            var retVal = new EditorialReview();
            retVal.Id = Id;
            retVal.CreatedBy = CreatedBy;
            retVal.CreatedDate = CreatedDate;
            retVal.ModifiedBy = ModifiedBy;
            retVal.ModifiedDate = ModifiedDate;

            retVal.Content = Content;
            retVal.ReviewType = ReviewType;
            retVal.LanguageCode = LanguageCode;
            retVal.IsInherited = IsInherited;
            return retVal;
        }
        #endregion
    }
}
