using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.TaxModule.Core.Model
{
    /// <summary>
    /// Represent base class for all potential tax calculation integrations
    /// </summary>
    public abstract class TaxProvider : Entity, IHasSettings, ICloneable
    {
        public string StoreId { get; set; }
        /// <summary>
        /// Method identity property (System name)
        /// </summary>
        public string Code { get; set; }

        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }

        public abstract IEnumerable<TaxRate> CalculateRates(IEvaluationContext context);
        public virtual string TypeName => GetType().Name;

        #region IHasSettings Members

        public ICollection<ObjectSettingEntry> Settings { get; set; }

        #endregion

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as TaxProvider;

            result.Settings = Settings?.Select(x => x.Clone()).OfType<ObjectSettingEntry>().ToList();

            return result;
        }

        #endregion
    }
}
