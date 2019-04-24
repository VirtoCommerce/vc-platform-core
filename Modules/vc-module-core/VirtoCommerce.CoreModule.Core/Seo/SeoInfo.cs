using System;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.CoreModule.Core.Seo
{
    public class SeoInfo : AuditableEntity, IHasLanguage, ICloneable
    {
        public SeoInfo()
        {
            IsActive = true;
        }
        public string Name { get; set; }
        /// <summary>
        /// Slug
        /// </summary>
        public string SemanticUrl { get; set; }
        /// <summary>
        /// head title tag content
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// <meta name="description" />
        /// </summary>
        public string MetaDescription { get; set; }
        public string ImageAltDescription { get; set; }
        /// <summary>
        /// <meta name="keywords" />
        /// </summary>
        public string MetaKeywords { get; set; }
        /// <summary>
        /// Tenant StoreId which SEO defined
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// SEO related object id
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// SEO related object type name
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// Active/Inactive
        /// </summary>
        public bool IsActive { get; set; }

        #region ILanguageSupport Members
        public string LanguageCode { get; set; }
        #endregion

        #region ICloneable members
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion


        #region Conditional JSON serialization for properties declared in base type
        public override bool ShouldSerializeAuditableProperties => false;
        #endregion
    }
}
