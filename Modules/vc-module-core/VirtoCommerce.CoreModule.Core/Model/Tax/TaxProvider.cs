using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CoreModule.Core.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.CoreModule.Core.Model.Tax
{
    /// <summary>
    /// Represent base class for all potential tax calculation integrations
    /// </summary>
    public abstract class TaxProvider : Entity, IHasSettings
    {
        /// <summary>
        /// Method identity property (System name)
        /// </summary>
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }

        public abstract IEnumerable<TaxRate> CalculateRates(IEvaluationContext context);
        public virtual string TypeName => GetType().Name;

        #region IHasSettings Members

        public ICollection<ObjectSettingEntry> Settings { get; set; }

        #endregion

    }
}
