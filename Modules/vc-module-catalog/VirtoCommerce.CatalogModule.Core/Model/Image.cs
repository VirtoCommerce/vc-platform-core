using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CatalogModule.Core.Model
{
    public class Image : AuditableEntity, ISeoSupport, IHasLanguage, ICloneable, IInheritable
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Group { get; set; }
        public int SortOrder { get; set; }

        public byte[] BinaryData { get; set; }

        #region IInheritable Members
        public bool IsInherited { get; set; }

        public virtual void TryInheritFrom(IEntity parent)
        {
            if (parent is Image parentBase)
            {
                Id = null;
                IsInherited = true;
                LanguageCode = parentBase.LanguageCode;
                Name = parentBase.Name;
                Group = parentBase.Group;
                Url = parentBase.Url;
                SortOrder = parentBase.SortOrder;
                BinaryData = parentBase.BinaryData;
            }
        }
        #endregion


        #region ISeoSupport Members
        public string SeoObjectType { get { return GetType().Name; } }
        public IList<SeoInfo> SeoInfos { get; set; }
        #endregion

        #region ILanguageSupport Members
        public string LanguageCode { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            var retVal = new Image();
            retVal.Id = Id;
            retVal.CreatedBy = CreatedBy;
            retVal.CreatedDate = CreatedDate;
            retVal.ModifiedBy = ModifiedBy;
            retVal.ModifiedDate = ModifiedDate;

            retVal.Name = Name;
            retVal.Url = Url;
            retVal.Group = Group;
            retVal.SortOrder = SortOrder;
            retVal.LanguageCode = LanguageCode;
            retVal.IsInherited = IsInherited;
            if (SeoInfos != null)
            {
                retVal.SeoInfos = SeoInfos.Select(x => x.Clone()).OfType<SeoInfo>().ToList();
            }
            return retVal;
        }
        #endregion
    }
}
