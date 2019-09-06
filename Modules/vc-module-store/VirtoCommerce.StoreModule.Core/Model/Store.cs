using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.StoreModule.Core.Model
{
    public class Store : AuditableEntity, IHasDynamicProperties, IHasSettings, ISeoSupport, ISupportSecurityScopes, IHasOuterId, ICloneable
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Store current state (Open, Closed, RestrictedAccess)
        /// </summary>
        public StoreState StoreState { get; set; }

        public string TimeZone { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string DefaultLanguage { get; set; }

        public string DefaultCurrency { get; set; }
        /// <summary>
        /// Catalog id used as primary store catalog
        /// </summary>
        public string Catalog { get; set; }
        public bool CreditCardSavePolicy { get; set; }
        /// <summary>
        /// Store storefront url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Store storefront https url
        /// </summary>
        public string SecureUrl { get; set; }
        /// <summary>
        /// Primary store contact email can be used for store event notifications and for feed back
        /// </summary>
        public string Email { get; set; }
        public string AdminEmail { get; set; }
        public bool DisplayOutOfStock { get; set; }
        public string OuterId { get; set; }

        /// <summary>
        /// Primary (default) fulfillment center id
        /// </summary>
        public string MainFulfillmentCenterId { get; set; }
        /// <summary>
        /// Alternate fulfillment centers ids
        /// </summary>
        public ICollection<string> AdditionalFulfillmentCenterIds { get; set; }
        /// <summary>
        /// Primary (default) fulfillment center for order return
        /// </summary>
        public string MainReturnsFulfillmentCenterId { get; set; }
        /// <summary>
        /// Alternate fulfillment centers for order return
        /// </summary>
        public ICollection<string> ReturnsFulfillmentCenterIds { get; set; }

        /// <summary>
        /// All store supported languages
        /// </summary>
        public ICollection<string> Languages { get; set; }
        /// <summary>
        /// All store supported currencies
        /// </summary>
        public ICollection<string> Currencies { get; set; }
        /// <summary>
        /// All store trusted groups (group of stores that shared the user logins)
        /// </summary>
        public ICollection<string> TrustedGroups { get; set; }


        #region ISeoSupport Members
        public string SeoObjectType { get { return GetType().Name; } }
        public IList<SeoInfo> SeoInfos { get; set; }
        #endregion

        #region IHasDynamicProperties Members
        public string ObjectType => GetType().FullName;
        public ICollection<DynamicObjectProperty> DynamicProperties { get; set; }
        #endregion

        #region IHaveSettings Members
        public ICollection<ObjectSettingEntry> Settings { get; set; }
        public virtual string TypeName => GetType().Name;
        #endregion

        #region ISupportSecurityScopes Members
        public IEnumerable<string> Scopes { get; set; }
        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Store;

                result.SeoInfos = SeoInfos?.Select(x => x.Clone()).OfType<SeoInfo>().ToList();
                result.Settings = Settings?.Select(x => x.Clone()).OfType<ObjectSettingEntry>().ToList();

            return result;
        }

        #endregion
    }
}
